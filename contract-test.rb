#!/usr/bin/env ruby
# Runs the generated contract cases (sdk/FilesContractTests) against the Files.com API simulator generated from the
# same schema, on every target framework, and fails unless each case the generator emitted was discovered and its
# result is accounted for:
#
#   ruby contract-test.rb MOCK_SERVER OUT [PACKAGE.nupkg PACKAGE_SHA256]
#
# MOCK_SERVER is a generated mock-server directory (simulation contract 3) with its bundle installed. The run binds it
# before anything is sent: the schema its lib/simulation/generation.json names must be the one these cases were
# generated from (sdk/FilesContractTests/contract-cases.json), and the simulator's readiness and inventory must agree.
# This script starts it on a loopback port and stops its whole process group at the end; with FILES_CONTRACT_MOCK_URL
# set, it uses that simulator, started from MOCK_SERVER by the caller, and binds it the same way.
#
# The simulator runs in its own bundle, MOCK_SERVER's Gemfile, never the caller's. Run under a caller's Bundler, as the
# generator's --run-tests route runs test.sh inside the generator's bundle exec, this script starts the simulator with
# Bundler's unbundled environment: neither the caller's Gemfile nor any BUNDLE_* setting reaches it, so its gems must be
# installed where its own Gemfile finds them, as bundle install in MOCK_SERVER does. Run outside Bundler, as test.sh is
# when called directly, the caller's environment, BUNDLE_* settings included, reaches it as it is. If the simulator stops
# or does not listen, the failure quotes the start and end of its output and names OUT/simulator.log, which keeps all of it.
#
# Without PACKAGE, the cases build against this SDK's FilesCom project. With it, they run against that exact archive
# (its SHA-256 checked), installed as an application installs it: FilesCom only from a feed holding that archive, with
# its own package folder, and every other package from FILES_CONTRACT_NUGET_SOURCES (nuget.org by default). Each run
# records the FilesCom assembly it loaded, which must then be the archive's for that framework.
#
# A case applies when the simulator's inventory declares its operation a record operation, or exactly the specialized
# operation contract-cases.json binds the case to (users' operations, a file migration's find): it must then pass, unless
# the case's own raw requests, never the SDK's, showed in the case's setup, before the SDK was called, that the
# simulator cannot serve it (a raw request refused as not simulated, or a record contradicting the schema), which is
# listed and never credited. One that does not apply must report the simulator's declaration and nothing else.
#
# The run fails unless the project targets one or more distinct frameworks and, on each, dotnet test exits 0 and every
# generated case is discovered once and reports one result. A missing, extra, repeated, skipped or failed case, a
# result for no discovered case, or a framework that executed none, fails. OUT receives each framework's TRX and the
# identity of the loaded SDK, the simulator's output, the restored dependency graph, and contract-results.json, the
# account of every case.
require "digest"
require "fileutils"
require "json"
require "net/http"
require "open3"
require "rexml/document"

DIR = __dir__
PROJECT = File.join(DIR, "sdk/FilesContractTests/FilesContractTests.csproj")
CASES = File.join(DIR, "sdk/FilesContractTests/contract-cases.json")
START_SECONDS = 60
STOP_SECONDS = 5
# A target framework moniker, such as net10.0 or net481.
FRAMEWORK = /\Anet(?:\d+\.\d+|\d{2,3})\z/

mock, out, package, package_sha256 = ARGV
abort "usage: contract-test.rb MOCK_SERVER OUT [PACKAGE.nupkg PACKAGE_SHA256]" unless mock && out && (package.nil? || package_sha256)
mock = File.expand_path(mock)
out = File.expand_path(out)
FileUtils.mkdir_p(out)
problems = []
report = { "cases_file" => { "path" => CASES, "sha256" => Digest::SHA256.file(CASES).hexdigest } }

def control(url, name)
  response = Net::HTTP.get_response(URI("#{url}/__files_mock/v1/#{name}"))
  raise "the simulator's #{name} answered #{response.code}" unless response.code == "200"

  JSON.parse(response.body)
end

# Whether any process of the simulator's group remains. Its leader, the simulator this script started, is reaped first if
# it has exited: until then it is still a member, which macOS reports as one that cannot be signalled (EPERM). EPERM
# from any member means the group is not empty.
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

# A bounded excerpt of a process's output for a message: all of a short one, and the start of a long one, where a process
# that could not start usually names why, with its end.
def excerpt(text, limit = 4000)
  text = text.to_s.strip
  return "(no output)" if text.empty?
  return text if text.size <= limit

  "#{text[0, limit / 2]}\n...\n#{text[-(limit / 2)..]}"
end

# The SHA-256 of an archive member, read with unzip, which every supported image has.
def member_sha256(archive, member)
  bytes, status = Open3.capture2("unzip", "-p", archive, member, binmode: true)
  status.success? && !bytes.empty? ? Digest::SHA256.hexdigest(bytes) : nil
end

cases = JSON.parse(File.read(CASES))
expected = cases.fetch("cases").to_h { |c| [ c.fetch("test"), c ] }
problems << "the cases name no case" if expected.empty?
receipt = File.join(mock, "lib/simulation/generation.json")
generation = File.exist?(receipt) ? JSON.parse(File.read(receipt)) : {}
report["binding"] = { "schema" => cases.fetch("schema"), "simulator_generation" => generation }
if generation.empty?
  problems << "#{mock} has no lib/simulation/generation.json: it is not a simulator the cases can be bound to (mock-server contract version 3)"
elsif generation.dig("swagger", "sha256") != cases.dig("schema", "sha256")
  problems << "the simulator was generated from schema #{generation.dig("swagger", "sha256")}, not the cases' #{cases.dig("schema", "sha256")}"
end

if package
  actual = Digest::SHA256.file(package).hexdigest
  problems << "#{package} has SHA-256 #{actual}, not #{package_sha256}" unless actual == package_sha256
  nuspec, = Open3.capture2("unzip", "-p", package, "FilesCom.nuspec")
  version = nuspec[%r{<version>([^<]+)</version>}, 1]
  problems << "#{package} is not a FilesCom package" unless nuspec.include?("<id>FilesCom</id>") && version
  report["package"] = { "path" => package, "sha256" => actual, "version" => version }
end

simulator = nil
url = ENV["FILES_CONTRACT_MOCK_URL"].to_s.chomp("/")
begin
  if problems.empty? && url.empty?
    log = File.open(File.join(out, "simulator.log"), "w")
    env = { "FILES_MOCK_MODE" => "simulation", "BUNDLE_GEMFILE" => File.join(mock, "Gemfile"), "RUBYOPT" => nil, "RUBYLIB" => nil }
    spawn = -> { Process.spawn(env, "bundle", "exec", "puma", "-b", "tcp://127.0.0.1:0", chdir: mock, in: File::NULL, out: log, err: log, pgroup: true) }
    pid = defined?(Bundler) ? Bundler.with_unbundled_env(&spawn) : spawn.call
    log.close
    simulator = { "pid" => pid, "pgid" => pid }
    deadline = Time.now + START_SECONDS
    until (port = File.read(log.path)[%r{Listening on http://127\.0\.0\.1:(\d+)}, 1])
      exited = Process.wait(pid, Process::WNOHANG) && $?
      if exited || Time.now > deadline
        output = File.read(log.path)
        what = exited ? "exited (#{exited.exitstatus ? "status #{exited.exitstatus}" : "signal #{exited.termsig}"}) before it listened" : "did not listen within #{START_SECONDS}s"
        if output.match?(/Bundler::GemNotFound|Could not find gem/)
          what += ". Its gems are not installed where its own Gemfile finds them"
          what += ", and it was started without the caller's Bundler settings, as under bundle exec" if defined?(Bundler)
          what += ": run bundle install in #{mock}"
        end
        raise "the simulator #{what}. Its output, all kept in #{log.path}:\n#{excerpt(output)}"
      end

      sleep 0.1
    end
    url = "http://127.0.0.1:#{port}"
  end

  if problems.empty?
    ready = control(url, "ready")
    inventory = control(url, "inventory")
    report["binding"]["simulator"] = ready.slice("mode", "contract_version", "simulator_version", "schema_sha256", "instance")
    problems << "the simulator is in #{ready["mode"].inspect} mode, not simulation" unless ready["mode"] == "simulation"
    problems << "the simulator's contract is #{ready["contract_version"].inspect}; the cases need contract version 3" unless ready["contract_version"] == 3
    problems << "the simulator validates schema subset #{ready["schema_sha256"]}, not its generation's #{generation["schema_sha256"]}" unless ready["schema_sha256"] == generation["schema_sha256"]
    problems << "the simulator's inventory is of schema #{inventory["swagger_sha256"]}, not the cases'" unless inventory["swagger_sha256"] == cases.dig("schema", "sha256")
  end

  if problems.empty?
    entries = inventory.fetch("entries").to_h { |entry| [ [ entry["model"], entry["method"] ], entry ] }
    applicable = expected.transform_values do |c|
      entry = entries[[ c["model"], c["method"] ]]
      next false unless entry

      entry["disposition"] == "record" ||
        (!c["specialized"].nil? && entry["disposition"] == "specialized" && entry["simulator_operation"] == c["specialized"])
    end
    unknown = expected.select { |_, c| entries[[ c["model"], c["method"] ]].nil? }.keys
    problems << "the simulator's inventory has no operation for #{unknown.size} cases: #{unknown.first(5).join(", ")}" if unknown.any?

    dotnet_env = { "FILES_CONTRACT_MOCK_URL" => url, "DOTNET_CLI_TELEMETRY_OPTOUT" => "1", "DOTNET_SKIP_FIRST_TIME_EXPERIENCE" => "1", "DOTNET_ROLL_FORWARD" => nil }
    properties = []
    if package
      # FilesCom only from the given archive, and from no package folder an earlier restore filled.
      work = File.join(out, "package")
      FileUtils.rm_rf(work)
      FileUtils.mkdir_p(File.join(work, "feed"))
      FileUtils.cp(package, File.join(work, "feed", "FilesCom.#{report["package"]["version"]}.nupkg"))
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
      properties << "-p:FilesComPackageVersion=#{report["package"]["version"]}"
    end

    frameworks, status = Open3.capture2(dotnet_env, "dotnet", "msbuild", PROJECT, "-getProperty:TargetFrameworks", *properties)
    raise "the target frameworks could not be read" unless status.success?

    # The frameworks the cases run on: one or more distinct monikers. Any other answer, an empty one above all, would
    # run nothing.
    report["target_frameworks"] = frameworks.strip
    selected = frameworks.strip.split(";", -1).map(&:strip)
    if frameworks.strip.empty?
      problems << "the project targets no framework: its TargetFrameworks is empty"
    elsif selected.any? { |framework| !framework.match?(FRAMEWORK) } || selected.uniq.size != selected.size
      problems << "the project's TargetFrameworks #{frameworks.strip.inspect} is not a list of distinct target frameworks"
    end

    report["frameworks"] = {}
    (problems.empty? ? selected : []).each do |framework|
      results = File.join(out, framework)
      FileUtils.rm_rf(results)
      FileUtils.mkdir_p(results)
      identity_file = File.join(results, "identity.json")
      started = Time.now.utc
      output, run = Open3.capture2e(dotnet_env.merge("FILES_CONTRACT_IDENTITY_FILE" => identity_file), "dotnet", "test", PROJECT, "--framework", framework,
                                    "--logger", "trx;LogFileName=#{framework}.trx", "--results-directory", results, *properties)
      File.write(File.join(results, "dotnet-test.log"), output)
      account = { "started" => started.iso8601, "seconds" => (Time.now.utc - started).round(1), "dotnet_test_exit" => run.exitstatus, "dotnet_test_signal" => run.termsig,
                  "passed" => [], "failed" => {}, "not_applicable" => {}, "simulator_limited" => {}, "missing" => [], "extra" => [], "unexpected" => {},
                  "multiplicity" => {}, "repeated_ids" => [], "results_for_no_case" => [] }
      trx = File.join(results, "#{framework}.trx")
      if File.exist?(trx)
        document = REXML::Document.new(File.read(trx))
        # Every case the run discovered and every result it reported, each occurrence counted: a case must be
        # discovered once and report one result, or which of its results counts would be a guess.
        names = {}
        definitions = Hash.new(0)
        REXML::XPath.each(document, "//UnitTest") do |test|
          method = test.elements["TestMethod"]
          name = "#{method.attributes["className"]}.#{method.attributes["name"]}"
          account["repeated_ids"] << test.attributes["id"] if names.key?(test.attributes["id"])
          names[test.attributes["id"]] = name
          definitions[name] += 1
        end
        outcomes = {}
        REXML::XPath.each(document, "//UnitTestResult") do |result|
          name = names[result.attributes["testId"]]
          if name.nil?
            account["results_for_no_case"] << result.attributes["testId"].to_s
            next
          end
          (outcomes[name] ||= []) << [ result.attributes["outcome"], result.elements["Output/ErrorInfo/Message"]&.text.to_s ]
        end
        account["discovered"] = definitions.values.sum
        account["results"] = outcomes.values.sum(&:size) + account["results_for_no_case"].size
        definitions.each_key do |test|
          counts = { "discovered" => definitions[test], "results" => outcomes.fetch(test, []).size }
          account["multiplicity"][test] = counts unless counts.values == [ 1, 1 ]
        end
        account["missing"] = expected.keys - definitions.keys
        account["extra"] = definitions.keys - expected.keys
        outcomes.each do |test, reported|
          next unless expected.key?(test) && !account["multiplicity"].key?(test)

          outcome, message = reported.first
          skipped = %w[Inconclusive NotExecuted].include?(outcome)
          if applicable[test] && outcome == "Passed"
            account["passed"] << test
          elsif applicable[test] && skipped && (reason = message[/not-applicable: in the case's setup, before the SDK was called, the simulator(?: refuses the case's raw|'s .* record contradicts the schema).*/])
            # The simulator could not serve the case, as the case's own raw requests showed in its setup, before the
            # SDK was called: a raw request refused as not simulated, or a record contradicting the schema. Never
            # credited. Once the SDK was called, the same refusal fails the case.
            account["simulator_limited"][test] = reason
          elsif applicable[test]
            account["failed"][test] = "#{outcome}: #{message}"
          elsif skipped && message.match?(/not-applicable: the simulator declares/)
            account["not_applicable"][test] = message[/not-applicable: .*/]
          else
            account["unexpected"][test] = "#{outcome} for a case the simulator does not declare a record operation: #{message}"
          end
        end
      else
        account["discovered"] = 0
      end
      account["executed"] = account["passed"].size + account["failed"].size
      # The dependency graph the restore resolved, each package with its SHA-512.
      assets = File.join(File.dirname(PROJECT), "obj", "project.assets.json")
      FileUtils.cp(assets, File.join(results, "project.assets.json")) if File.exist?(assets)
      account["identity"] = File.exist?(identity_file) ? JSON.parse(File.read(identity_file)) : nil
      label = "#{framework}:"
      problems << "#{label} dotnet test #{run.exitstatus ? "exited #{run.exitstatus}" : "was ended by signal #{run.termsig}"}, whatever its results say" unless run.success?
      problems << "#{label} no TRX was written (dotnet test exit #{run.exitstatus})" unless File.exist?(trx)
      problems << "#{label} no case executed" if account["executed"].zero?
      problems << "#{label} #{account["missing"].size} generated cases were not discovered: #{account["missing"].first(5).join(", ")}" if account["missing"].any?
      problems << "#{label} #{account["extra"].size} cases were not generated: #{account["extra"].first(5).join(", ")}" if account["extra"].any?
      if account["multiplicity"].any?
        repeated = account["multiplicity"].first(5).map { |test, counts| "#{test} (discovered #{counts["discovered"]}, results #{counts["results"]})" }
        problems << "#{label} #{account["multiplicity"].size} cases were not discovered once with one result: #{repeated.join(", ")}"
      end
      problems << "#{label} #{account["repeated_ids"].size} test IDs were discovered more than once: #{account["repeated_ids"].first(5).join(", ")}" if account["repeated_ids"].any?
      problems << "#{label} #{account["results_for_no_case"].size} results name no discovered case: #{account["results_for_no_case"].first(5).join(", ")}" if account["results_for_no_case"].any?
      problems << "#{label} #{account["failed"].size} applicable cases did not pass: #{account["failed"].keys.first(5).join(", ")}" if account["failed"].any?
      problems << "#{label} #{account["unexpected"].size} cases the simulator does not apply reported otherwise: #{account["unexpected"].keys.first(5).join(", ")}" if account["unexpected"].any?
      if account["identity"].nil?
        problems << "#{label} the cases did not record the FilesCom assembly they loaded"
      elsif package
        moniker = account["identity"]["target_framework"].to_s.sub(".NETCoreApp,Version=v", "net")
        archived = member_sha256(package, "lib/#{moniker}/FilesCom.dll")
        account["identity"]["package_member"] = { "path" => "lib/#{moniker}/FilesCom.dll", "sha256" => archived }
        problems << "#{label} loaded FilesCom #{account["identity"]["sha256"]}, not the package's lib/#{moniker}/FilesCom.dll #{archived.inspect}" unless archived && archived == account["identity"]["sha256"]
      end
      report["frameworks"][framework] = account
      puts "#{framework}: dotnet test exit #{run.exitstatus.inspect}, #{account["discovered"]} cases discovered, #{account["results"].to_i} results, #{account["executed"]} executed, #{account["passed"].size} passed, " \
           "#{account["failed"].size} failed, #{account["not_applicable"].size} not applicable (declared by the simulator), " \
           "#{account["simulator_limited"].size} not served by the simulator (shown by the case's own raw requests)"
    end
  end
rescue StandardError => e
  problems << "#{e.class}: #{e.message}"
ensure
  if simulator
    pgid = simulator["pgid"]
    stop = "exited"
    if group_alive?(pgid)
      Process.kill("TERM", -pgid)
      stop = "TERM"
      deadline = Time.now + STOP_SECONDS
      sleep 0.1 while group_alive?(pgid) && Time.now < deadline
      if group_alive?(pgid)
        Process.kill("KILL", -pgid)
        stop = "KILL"
        sleep 0.5
      end
    end
    begin
      Process.wait(simulator["pid"])
    rescue Errno::ECHILD
      nil
    end
    simulator["stop"] = stop
    simulator["group_empty"] = !group_alive?(pgid)
    problems << "the simulator's process group did not empty" unless simulator["group_empty"]
    report["simulator_process"] = simulator
  end
  report["problems"] = problems
  report["result"] = problems.empty? ? "pass" : "fail"
  File.write(File.join(out, "contract-results.json"), "#{JSON.pretty_generate(report)}\n")
  problems.each { |problem| warn "contract-test: #{problem}" }
  puts "contract-test: #{report["result"]}; #{File.join(out, "contract-results.json")}"
end
exit(problems.empty? ? 0 : 1)
