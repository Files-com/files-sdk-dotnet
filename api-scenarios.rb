#!/usr/bin/env ruby
# Runs the generated API scenario tests (sdk/FilesApiScenarioTests) against the exact FilesCom archive given, on every
# target framework, one case at a time, each against a fresh simulator that the generated mock server's with-scenario.rb
# starts for it under the api-scenarios-canonical preset, and fails unless the controls and every case pass:
#
#   ruby api-scenarios.rb MOCK_SERVER OUT PACKAGE.nupkg PACKAGE_SHA256
#
# MOCK_SERVER is a generated mock-server directory (simulation contract 3) with its bundle installed, generated from the
# schema the cases were (sdk/FilesApiScenarioTests/api-scenarios.json): its lib/simulation/generation.json must record that
# raw schema, and its api-scenarios-canonical preset the cases' definition, with serial uploads of fixed part lengths. OUT
# must be absent or empty, and outside this SDK, which is published as it is (checked, links followed, before OUT is
# made); it receives everything, failed or not.
#
# The archive (its SHA-256 checked) is installed as an application installs it: FilesCom only from a feed holding it, with
# a package folder of its own, and every other package from FILES_CONTRACT_NUGET_SOURCES, as the record contract cases
# take them (nuget.org by default). On each framework the project is restored and built once, before any case's time
# starts. Its controls run next, then each case alone under the launcher, with the bounds selected for these cases: 30
# seconds for the simulator to start, 210 for the case, 5 to stop each process group, and a 16 MiB body limit for the
# simulator, which lets the preset grant its 5 MiB parts. A launcher still running after its own bound, or when this run is
# interrupted (INT, TERM or HUP), is stopped with its process group, TERM first, so that it stops what it started and
# saves the journal; the case's and the simulator's process groups its run.json records are then checked, and stopped if
# anything of them is left. An interrupted run records the interruption as a failure, writes its results, and then ends
# as that signal; it never reports a pass.
#
# A case passes only when the launcher exits 0 (the case passed, the journal is complete and every process group it started
# is gone), the TRX shows that one test once with one passed result, the case's receipt shows every simulator position
# passed and exactly the manifest's site-only positions excluded, with its requests joined to the same journal entries the
# launcher saved, and the FilesCom assembly the case loaded is the archive's own for that framework. Every case runs,
# whatever failed before it; the first failure is kept. OUT/api-scenarios-results.json accounts for all of it.
require "digest"
require "fileutils"
require "json"
require "open3"
require "rbconfig"
require "rexml/document"
require "securerandom"
require "time"

DIR = __dir__
PROJECT = File.join(DIR, "sdk/FilesApiScenarioTests/FilesApiScenarioTests.csproj")
MANIFEST = File.join(DIR, "sdk/FilesApiScenarioTests/api-scenarios.json")
PRESET = "api-scenarios-canonical".freeze
CONTROLS = "TestCategory=ScenarioControls".freeze
STARTUP_SECONDS = 30
CONSUMER_SECONDS = 210
SETTLE_SECONDS = 5
# A case's own bounds (ScenarioRuntime.cs), recorded with the results.
CASE_SECONDS = { "work" => 175, "cleanup" => 30, "consumer" => 205 }.freeze
# The launcher's own bound: the simulator's start, the case, then stopping the case's and the simulator's process groups
# (TERM, then KILL, each waited for up to SETTLE_SECONDS) around saving the journal, with room for its bookkeeping.
LAUNCHER_SECONDS = STARTUP_SECONDS + CONSUMER_SECONDS + (4 * SETTLE_SECONDS) + 30
# What a launcher sent TERM is given to stop what it started and save the journal, before KILL: that same cleanup.
LAUNCHER_CLEANUP_SECONDS = (4 * SETTLE_SECONDS) + 30
# INT, TERM and HUP end a run as that signal, once what it owns is settled and its results are written: they are raised
# where the run is, except while a case's launcher is spawned, waited for and settled (run_launcher) or the results
# written, when they are held until then.
SIGNALS = %w[INT TERM HUP].freeze
MOCK_MAX_BODY_BYTES = 16 * 1024 * 1024
# A target framework moniker, such as net10.0.
FRAMEWORK = /\Anet(?:\d+\.\d+|\d{2,3})\z/

def read_json(path)
  [ JSON.parse(File.read(path)), nil ]
rescue JSON::ParserError, SystemCallError => e
  [ nil, "#{e.class}: #{e.message}" ]
end

# The SHA-256 of an archive member, read with unzip, which every supported image has.
def member_sha256(archive, member)
  bytes, status = Open3.capture2("unzip", "-p", archive, member, binmode: true)
  status.success? && !bytes.empty? ? Digest::SHA256.hexdigest(bytes) : nil
end

# The archive's FilesCom.dll for the framework a run loaded, and why not, unless the run loaded exactly it.
def package_member(package, identity)
  return [ nil, "the run did not record the FilesCom assembly it loaded" ] unless identity.is_a?(Hash)

  moniker = identity["target_framework"].to_s.sub(".NETCoreApp,Version=v", "net")
  member = "lib/#{moniker}/FilesCom.dll"
  archived = member_sha256(package, member)
  record = { "path" => member, "sha256" => archived }
  return [ record, nil ] if archived && archived == identity["sha256"]

  [ record, "it loaded FilesCom #{identity["sha256"].inspect}, not the package's #{member} #{archived.inspect}" ]
end

# Every test a TRX discovered, each occurrence counted, and every result it reported.
def trx_account(path)
  return nil unless File.exist?(path)

  document = REXML::Document.new(File.read(path))
  tests = {}
  REXML::XPath.each(document, "//UnitTest") do |test|
    method = test.elements["TestMethod"]
    tests[test.attributes["id"]] = "#{method.attributes["className"]}.#{method.attributes["name"]}"
  end
  results = REXML::XPath.match(document, "//UnitTestResult").map do |result|
    message = result.elements["Output/ErrorInfo/Message"]&.text || ""
    { "test" => tests[result.attributes["testId"]], "name" => result.attributes["testName"], "outcome" => result.attributes["outcome"], "message" => message[0, 4000] }
  end
  { "discovered" => tests.values, "results" => results }
end

# Whether any process of the group remains. Its leader, a child of this script, is reaped first if it has exited, since
# until then it is still a member; EPERM from any member means the group is not empty.
def group_alive?(pgid)
  begin
    Process.wait(pgid, Process::WNOHANG)
  rescue Errno::ECHILD
    nil
  end
  Process.kill(0, -pgid)
  true
rescue Errno::EPERM
  true
rescue Errno::ESRCH
  false
end

# Stops what is left of an owned process group: TERM, then KILL, each waited for up to seconds.
def settle(pgid, seconds)
  signalled = nil
  %w[TERM KILL].each do |signal|
    break unless group_alive?(pgid)

    begin
      Process.kill(signal, -pgid)
    rescue Errno::ESRCH, Errno::EPERM
      nil
    end
    signalled = signal
    deadline = Process.clock_gettime(Process::CLOCK_MONOTONIC) + seconds
    sleep 0.1 while group_alive?(pgid) && Process.clock_gettime(Process::CLOCK_MONOTONIC) < deadline
  end
  { "signal" => signalled, "empty" => !group_alive?(pgid) }
end

def clock
  Process.clock_gettime(Process::CLOCK_MONOTONIC)
end

def signal_exception(signal)
  signal == "INT" ? Interrupt.new : SignalException.new(signal)
end

def signal_name(exception)
  "SIG#{Signal.signame(exception.signo)}"
end

def raise_signals
  SIGNALS.each { |signal| trap(signal) { raise signal_exception(signal) } }
end

# Runs the block with INT, TERM and HUP held; returns its value and the first of them that came meanwhile, or nil.
def holding_signals
  held = nil
  SIGNALS.each { |signal| trap(signal) { held ||= signal } }
  value = yield
  raise_signals
  [ value, held ]
ensure
  raise_signals
end

# The case's and the simulator's process groups, as the launcher's run.json records them, each settled if anything of it
# is left. The launcher stops them itself; this finishes what a launcher that was killed, or could not finish its
# cleanup, left behind. Without run.json they cannot be named, and so are not verified.
def settle_descendants(launcher_out)
  record, error = read_json(File.join(launcher_out, "run.json"))
  return { "verified" => false, "error" => "run.json: #{error || "not a JSON object"}" } unless record.is_a?(Hash)

  groups = { "consumer" => record.dig("consumer", "process_group"), "server" => record.dig("server", "process_group") }
  groups.select { |_, pgid| pgid.is_a?(Integer) && pgid.positive? }
        .to_h { |name, pgid| [ name, { "process_group" => pgid }.merge(settle(pgid, SETTLE_SECONDS)) ] }
        .merge("verified" => true)
end

# Runs the launcher in a process group of its own, up to LAUNCHER_SECONDS, then settles that group and the case's and
# the simulator's groups it records. INT, TERM and HUP are held from before the launcher is spawned until all of that is
# settled, so none can come between the spawn and its process ID being kept as this run's own: one that comes ends the
# wait. A launcher still running, after its bound or because this run was interrupted, is sent TERM first, which makes it
# stop what it started and save the journal (with-scenario.rb), and is given LAUNCHER_CLEANUP_SECONDS for that before
# KILL. However the wait ends (the launcher's exit, its bound, a signal or a failure), the groups are settled exactly
# once, and only when this run spawned the launcher; nothing else is signalled. Returns the launch's record and the
# interruption, if any, which the caller raises once the case's evidence is recorded; any other failure is raised once
# the groups are settled.
def run_launcher(env, command, log, launcher_out)
  started = clock
  pid = nil
  status = nil
  failure = nil
  held = nil
  settled = nil
  SIGNALS.each { |signal| trap(signal) { held ||= signal } }
  begin
    File.open(log, "w") { |file| pid = Process.spawn(env, *command, in: File::NULL, out: file, err: file, pgroup: true) }
    until held || (status = Process.wait2(pid, Process::WNOHANG)&.last)
      break if clock - started >= LAUNCHER_SECONDS

      sleep 0.2
    end
  rescue SystemCallError => e
    failure = e
  ensure
    settled = if pid
                { "launcher" => settle(pid, status ? SETTLE_SECONDS : LAUNCHER_CLEANUP_SECONDS), "descendants" => settle_descendants(launcher_out) }
              else
                { "launcher" => { "empty" => nil }, "descendants" => { "verified" => false, "error" => "the launcher was not started" } }
              end
    raise_signals
  end
  interruption = signal_exception(held) if held
  record = if failure
             { "exit" => nil, "error" => "#{failure.class}: #{failure.message}" }
           else
             { "exit" => status&.exitstatus, "signal" => status&.termsig, "timed_out" => status.nil? && interruption.nil? }
           end
  record.update("interrupted" => (signal_name(interruption) if interruption), "seconds" => (clock - started).round(1),
                "process_group" => settled["launcher"], "descendant_groups" => settled["descendants"])
  [ record, interruption ]
end

# PATH with links followed as far as it exists: the real path of its nearest existing ancestor, then the names below it.
def resolved_path(path)
  names = []
  until File.exist?(path)
    names.unshift(File.basename(path))
    path = File.dirname(path)
  end
  File.join(File.realpath(path), *names)
end

mock, out, package, package_sha256 = ARGV
abort "usage: ruby api-scenarios.rb MOCK_SERVER OUT PACKAGE.nupkg PACKAGE_SHA256" unless ARGV.size == 4

mock = File.expand_path(mock)
out = File.expand_path(out)
package = File.expand_path(package)
sdk = File.realpath(DIR)
inside_sdk = ->(path) { path == sdk || path.start_with?("#{sdk}/") }
# Before anything is made, with links followed as far as OUT exists; and again once it does.
abort "api-scenarios: OUT #{out} is inside #{sdk}, which is published as it is" if inside_sdk.(resolved_path(out))
abort "api-scenarios: OUT #{out} must be absent or an empty directory" if File.exist?(out) && !(File.directory?(out) && Dir.empty?(out))
FileUtils.mkdir_p(out)
abort "api-scenarios: OUT #{out} is inside #{sdk}, which is published as it is" if inside_sdk.(File.realpath(out))

raise_signals
problems = []
first_failure = nil
interrupted = nil
fail_with = lambda do |problem|
  problems << problem
  first_failure ||= problem
end
report = {
  "format" => "files-dotnet-api-scenarios-results", "version" => 1, "started" => Time.now.utc.iso8601, "out" => out,
  "bounds" => { "startup_seconds" => STARTUP_SECONDS, "consumer_seconds" => CONSUMER_SECONDS, "settle_seconds" => SETTLE_SECONDS,
                "launcher_seconds" => LAUNCHER_SECONDS, "mock_max_body_bytes" => MOCK_MAX_BODY_BYTES, "case_seconds" => CASE_SECONDS },
  "frameworks" => {},
}

begin
  manifest, error = read_json(MANIFEST)
  cases = []
  if manifest.is_a?(Hash) && manifest["cases"].is_a?(Array)
    cases = manifest["cases"]
    report["manifest"] = { "path" => MANIFEST, "sha256" => Digest::SHA256.file(MANIFEST).hexdigest }.merge(manifest.slice("definition", "schema", "profile"))
    fail_with.("the manifest names no case") if cases.empty?
    fail_with.("the manifest's preset is #{manifest["profile"].inspect}, not #{PRESET}") unless manifest["profile"] == PRESET
    tests = cases.map { |c| c["test"] }
    fail_with.("the manifest's tests are not distinct") unless tests.uniq.size == tests.size
    cases.each do |c|
      complete = c["id"].is_a?(String) && c["test"].to_s.start_with?("#{manifest["tests"]}.") && c["positions"].is_a?(Integer) &&
                 c["simulator"].is_a?(Integer) && c["site_only"].is_a?(Array)
      fail_with.("the manifest's case #{c["id"].inspect} is incomplete") unless complete
    end
  else
    fail_with.("the manifest #{MANIFEST} cannot be read: #{error || "it names no cases"}")
    manifest = {}
  end

  # The simulator: the launcher, the raw schema its generation records and the preset the cases run under.
  launcher = File.join(mock, "with-scenario.rb")
  generation_path = File.join(mock, "lib/simulation/generation.json")
  preset_path = File.join(mock, "scenarios", "#{PRESET}.json")
  fail_with.("#{mock} has no with-scenario.rb: it is not a generated mock server") unless File.file?(launcher)
  generation, error = read_json(generation_path)
  if !generation.is_a?(Hash)
    fail_with.("#{generation_path} cannot be read (#{error}): #{mock} is not a simulator these cases can be bound to (mock-server contract version 3)")
  elsif generation.dig("swagger", "sha256") != manifest.dig("schema", "sha256")
    fail_with.("the simulator was generated from schema #{generation.dig("swagger", "sha256").inspect}, not the cases' #{manifest.dig("schema", "sha256").inspect}")
  end
  preset, error = read_json(preset_path)
  if !preset.is_a?(Hash)
    fail_with.("the #{PRESET} preset #{preset_path} cannot be read: #{error}")
  else
    profile = preset.dig("reset", "profile") || {}
    if profile.dig("api_scenarios", "definition_sha256") != manifest.dig("definition", "sha256")
      fail_with.("the #{PRESET} preset is for definition #{profile.dig("api_scenarios", "definition_sha256").inspect}, not the cases' #{manifest.dig("definition", "sha256").inspect}")
    end
    unless profile.dig("upload", "mode") == "serial" && profile.dig("upload", "fixed_part_length") == true
      fail_with.("the #{PRESET} preset's uploads are not serial with fixed part lengths: #{profile["upload"].inspect}")
    end
  end
  report["simulator"] = {
    "directory" => mock,
    "generation" => { "path" => generation_path, "sha256" => (Digest::SHA256.file(generation_path).hexdigest if File.file?(generation_path)),
                      "schema_sha256" => generation.is_a?(Hash) ? generation["schema_sha256"] : nil,
                      "swagger_sha256" => generation.is_a?(Hash) ? generation.dig("swagger", "sha256") : nil },
    "preset" => { "path" => preset_path, "sha256" => (Digest::SHA256.file(preset_path).hexdigest if File.file?(preset_path)) },
  }

  # The package: exactly the archive given, installed from a feed of its own.
  version = nil
  if File.file?(package)
    actual = Digest::SHA256.file(package).hexdigest
    fail_with.("#{package} has SHA-256 #{actual}, not #{package_sha256}") unless actual == package_sha256
    nuspec, = Open3.capture2("unzip", "-p", package, "FilesCom.nuspec")
    version = nuspec[%r{<version>([^<]+)</version>}, 1]
    fail_with.("#{package} is not a FilesCom package") unless nuspec.include?("<id>FilesCom</id>") && version
    report["package"] = { "path" => package, "sha256" => actual, "version" => version }
  else
    fail_with.("#{package} is not a file")
  end

  dotnet_env = { "DOTNET_CLI_TELEMETRY_OPTOUT" => "1", "DOTNET_SKIP_FIRST_TIME_EXPERIENCE" => "1", "DOTNET_ROLL_FORWARD" => nil }
  properties = []
  selected = []
  if problems.empty?
    work = File.join(out, "package")
    FileUtils.mkdir_p(File.join(work, "feed"))
    FileUtils.cp(package, File.join(work, "feed", "FilesCom.#{version}.nupkg"))
    sources = ENV.fetch("FILES_CONTRACT_NUGET_SOURCES", "https://api.nuget.org/v3/index.json").split(";").map(&:strip).reject(&:empty?)
    File.write(File.join(work, "NuGet.Config"), <<~CONFIG)
      <?xml version="1.0" encoding="utf-8"?>
      <configuration>
        <packageSources>
          <clear />
          <add key="package" value="#{File.join(work, "feed")}" />
      #{sources.each_with_index.map { |source, i| "    <add key=\"source#{i}\" value=\"#{source}\" />" }.join("\n")}
        </packageSources>
        <packageSourceMapping>
          <packageSource key="package"><package pattern="FilesCom" /></packageSource>
      #{sources.each_index.map { |i| "    <packageSource key=\"source#{i}\"><package pattern=\"*\" /></packageSource>" }.join("\n")}
        </packageSourceMapping>
      </configuration>
    CONFIG
    dotnet_env.merge!("RestoreConfigFile" => File.join(work, "NuGet.Config"), "NUGET_PACKAGES" => File.join(work, "packages"))
    properties << "-p:FilesComPackageVersion=#{version}"

    frameworks, status = Open3.capture2(dotnet_env, "dotnet", "msbuild", PROJECT, "-getProperty:TargetFrameworks", *properties)
    report["target_frameworks"] = frameworks.strip
    selected = frameworks.strip.split(";", -1).map(&:strip)
    if !status.success? || frameworks.strip.empty? || selected.any? { |framework| !framework.match?(FRAMEWORK) } || selected.uniq.size != selected.size
      fail_with.("the project's TargetFrameworks #{frameworks.strip.inspect} is not a list of distinct target frameworks")
      selected = []
    end
  end

  selected.each do |framework|
    directory = File.join(out, framework)
    FileUtils.mkdir_p(directory)
    account = report["frameworks"][framework] = { "cases" => {} }

    # Restored and built once, before any case's time starts.
    output, built = Open3.capture2e(dotnet_env, "dotnet", "build", PROJECT, "--framework", framework, *properties)
    File.write(File.join(directory, "build.log"), output)
    account["build"] = { "exit" => built.exitstatus, "signal" => built.termsig }
    assets = File.join(File.dirname(PROJECT), "obj", "project.assets.json")
    FileUtils.cp(assets, File.join(directory, "project.assets.json")) if File.exist?(assets)
    unless built.success?
      fail_with.("#{framework}: the API scenario tests did not build (exit #{built.exitstatus.inspect}); see #{File.join(directory, "build.log")}")
      cases.each { |c| account["cases"][c["id"]] = { "result" => "not run: the tests did not build" } }
      next
    end

    # The controls: the consumer's own decisions, against stubs at the network and data boundaries.
    controls = File.join(directory, "controls")
    FileUtils.mkdir_p(controls)
    identity_file = File.join(controls, "identity.json")
    output, ran = Open3.capture2e(dotnet_env.merge("FILES_CONTRACT_IDENTITY_FILE" => identity_file), "dotnet", "test", PROJECT, "--framework", framework, "--no-build",
                                  "--filter", CONTROLS, "--logger", "trx;LogFileName=controls.trx", "--results-directory", controls, *properties)
    File.write(File.join(controls, "dotnet-test.log"), output)
    trx = trx_account(File.join(controls, "controls.trx"))
    identity, = read_json(identity_file) if File.exist?(identity_file)
    member, mismatch = package_member(package, identity)
    account["controls"] = { "dotnet_test_exit" => ran.exitstatus, "dotnet_test_signal" => ran.termsig, "trx" => trx, "identity" => identity, "package_member" => member }
    label = "#{framework} controls"
    fail_with.("#{label}: dotnet test #{ran.exitstatus ? "exited #{ran.exitstatus}" : "was ended by signal #{ran.termsig}"}") unless ran.success?
    if trx.nil? || trx["results"].empty?
      fail_with.("#{label}: no control ran")
    else
      others = trx["discovered"].reject { |test| test.start_with?("#{manifest["controls"]}.") }
      fail_with.("#{label}: the filter ran tests that are no controls: #{others.first(5).join(", ")}") if others.any?
      failed = trx["results"].reject { |result| result["outcome"] == "Passed" }
      if failed.any?
        fail_with.("#{label}: #{failed.size} controls did not pass: #{failed.first(3).map { |result| "#{result["name"]} (#{result["outcome"]}: #{result["message"][0, 300]})" }.join("; ")}")
      end
    end
    fail_with.("#{label}: #{mismatch}") if mismatch

    # Each case alone, against a simulator of its own.
    cases.each_with_index do |c, index|
      case_directory = File.join(directory, c["id"])
      launcher_out = File.join(case_directory, "launcher")
      FileUtils.mkdir_p(case_directory)
      receipt_file = File.join(case_directory, "receipt.json")
      identity_file = File.join(case_directory, "identity.json")
      command = [
        RbConfig.ruby, launcher, "--scenario", PRESET, "--out", launcher_out, "--run-id", "dotnet-#{framework}-#{index + 1}-#{SecureRandom.hex(3)}",
        "--startup-timeout", STARTUP_SECONDS.to_s, "--consumer-timeout", CONSUMER_SECONDS.to_s, "--settle-timeout", SETTLE_SECONDS.to_s, "--",
        "dotnet", "test", PROJECT, "--framework", framework, "--no-build", "--filter", "FullyQualifiedName=#{c["test"]}",
        "--logger", "trx;LogFileName=case.trx", "--results-directory", case_directory, *properties
      ]
      env = dotnet_env.merge(
        "FILES_MOCK_MAX_BODY_BYTES" => MOCK_MAX_BODY_BYTES.to_s,
        "FILES_API_SCENARIOS_CASE" => c["id"],
        "FILES_API_SCENARIOS_RECEIPT" => receipt_file,
        "FILES_API_SCENARIOS_GENERATION" => generation_path,
        "FILES_CONTRACT_IDENTITY_FILE" => identity_file
      )
      launch, interruption = run_launcher(env, command, File.join(case_directory, "launcher.log"), launcher_out)

      found = []
      if launch["interrupted"]
        found << "the run was interrupted by #{launch["interrupted"]}, and the case's launcher was stopped"
      elsif !launch["exit"]&.zero?
        found << (launch["timed_out"] ? "the launcher was still running after #{LAUNCHER_SECONDS} s" : "the launcher exited #{launch["exit"].inspect}#{" (signal #{launch["signal"]})" if launch["signal"]}#{" (#{launch["error"]})" if launch["error"]}")
      end
      found << "the launcher's process group was not empty after TERM and KILL" unless launch.dig("process_group", "empty")
      descendants = launch["descendant_groups"] || {}
      if descendants["verified"] != true
        found << "the case's and the simulator's process groups could not be verified (#{descendants["error"] || "the launcher did not run"})"
      else
        descendants.each do |name, group|
          next unless group.is_a?(Hash)

          found << "the launcher left its #{name}'s process group #{group["process_group"]} running; it was stopped with #{group["signal"]}" if group["signal"]
          found << "the #{name}'s process group #{group["process_group"]} was not empty after TERM and KILL" unless group["empty"]
        end
      end
      run_record, = read_json(File.join(launcher_out, "run.json"))
      journal, = read_json(File.join(launcher_out, "journal.json"))
      receipt, = read_json(receipt_file)
      identity, = read_json(identity_file)
      trx = trx_account(File.join(case_directory, "case.trx"))
      entries = journal.is_a?(Hash) && journal["entries"].is_a?(Array) ? journal["entries"] : nil
      if entries.nil?
        found << "the launcher saved no journal"
      elsif !(journal["complete"] == true && journal["dropped"] == 0)
        found << "the journal is not complete (complete #{journal["complete"].inspect}, dropped #{journal["dropped"].inspect})"
      end
      if trx.nil?
        found << "no TRX was written"
      else
        found << "the run discovered #{trx["discovered"].inspect}, not #{c["test"]} alone" unless trx["discovered"] == [ c["test"] ]
        outcomes = trx["results"].map { |result| result["outcome"] }
        unless outcomes == [ "Passed" ]
          messages = trx["results"].map { |result| result["message"].to_s[0, 500] }.reject(&:empty?)
          found << "the test reported #{outcomes.inspect}, not one passed result#{": #{messages.join(" | ")}" if messages.any?}"
        end
      end
      if receipt.is_a?(Hash)
        counts = receipt["counts"] || {}
        found << "the receipt is of #{receipt["case"].inspect}, not #{c["id"]}" unless receipt["case"] == c["id"]
        found << "the receipt's definition is #{receipt["definition_sha256"].inspect}" unless receipt["definition_sha256"] == manifest.dig("definition", "sha256")
        found << "the case reported #{receipt["result"].inspect}: #{receipt.dig("first_failure", "message")}" unless receipt["result"] == "passed"
        unless counts["positions"] == c["positions"] && counts["simulator"] == c["simulator"] && counts["passed"] == c["simulator"] && counts["failed"] == 0 && counts["unreached"] == 0
          found << "the receipt counts #{counts.to_json}, not #{c["simulator"]} of #{c["positions"]} positions passed"
        end
        found << "the receipt excludes positions #{receipt["excluded_positions"].inspect}, not the site-only #{c["site_only"].inspect}" unless receipt["excluded_positions"] == c["site_only"]
        baseline = receipt.dig("journal", "baseline_seq").to_i
        saved = entries&.count { |entry| entry.is_a?(Hash) && entry["seq"].to_i > baseline }
        unless receipt.dig("journal", "final") == true && receipt.dig("journal", "joined") == saved && receipt.dig("journal", "entries") == saved
          found << "the case joined #{receipt.dig("journal", "joined").inspect} requests to its final journal, but the launcher saved #{saved.inspect} entries of the case"
        end
      else
        found << "the case wrote no receipt"
      end
      member, mismatch = package_member(package, identity)
      found << mismatch if mismatch

      label = "#{framework} #{c["id"]}"
      found.each { |problem| fail_with.("#{label}: #{problem}") }
      account["cases"][c["id"]] = {
        "result" => found.empty? ? "passed" : "failed", "test" => c["test"], "launcher" => launch, "run" => run_record, "receipt" => receipt_file,
        "counts" => receipt.is_a?(Hash) ? receipt["counts"] : nil, "first_failure" => receipt.is_a?(Hash) ? receipt["first_failure"] : nil,
        "cleanup" => receipt.is_a?(Hash) ? receipt["cleanup"]&.slice("outcome") : nil, "identity" => identity, "package_member" => member,
        "trx" => trx, "problems" => found
      }
      passed = receipt.is_a?(Hash) ? receipt.dig("counts", "passed") : nil
      puts "#{label}: #{found.empty? ? "passed" : "failed"} (#{passed.inspect} of #{c["simulator"]} simulator positions passed, #{c["site_only"].size} site-only positions excluded)"
      # The case's evidence is recorded and its launcher settled: the interruption goes on.
      raise interruption if interruption
    end
  end
rescue SignalException => e
  interrupted = e
  fail_with.("interrupted by #{signal_name(e)}")
rescue StandardError => e
  fail_with.("#{e.class}: #{e.message}")
ensure
  # Written however the run ended, with any signal that comes meanwhile held, then recorded and written too: an
  # interrupted run is never a pass.
  write_results = lambda do
    report["problems"] = problems
    report["first_failure"] = first_failure
    report["interrupted"] = (signal_name(interrupted) if interrupted)
    report["result"] = problems.empty? ? "pass" : "fail"
    report["finished"] = Time.now.utc.iso8601
    File.write(File.join(out, "api-scenarios-results.json"), "#{JSON.pretty_generate(report)}\n")
  end
  _, held = holding_signals { write_results.call }
  if held
    interrupted ||= signal_exception(held)
    fail_with.("interrupted by SIG#{held}")
    holding_signals { write_results.call }
  end
  problems.each { |problem| warn "api-scenarios: #{problem}" }
  puts "api-scenarios: #{report["result"]}; #{File.join(out, "api-scenarios-results.json")}"
end
# An interrupted run ends as its signal, once what it owned is settled and its results are written.
raise interrupted if interrupted
exit(problems.empty? ? 0 : 1)
