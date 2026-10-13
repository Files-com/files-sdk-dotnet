#!/usr/bin/env ruby
# The control for package-provenance.rb, which build.sh runs to keep its package with its provenance. It runs the real
# helper as build.sh does, as a separate ruby process in a generated project's directory, with an environment it sets in
# full, against inputs it makes itself, and checks what the helper keeps and what it refuses. It also runs the real
# build.sh, with a stand-in dotnet, to check that the recipe keeps a package only when every command's exit was recorded,
# and that the artifact directory is checked before any SDK command runs:
#
#   ruby package-provenance-test.rb OUT [--only CASE[,CASE...]]
#
# OUT must not exist. Everything is made inside it: OUT/source, a throw-away git checkout shaped like generated/dotnet,
# and for each case OUT/cases/CASE, with a stand-in package written by the small ZIP writer below (stored and deflated
# members), a schema capture tuple shaped as .gitlab-ci/capture-schema.sh writes it, the recipe's steps, and the artifact
# directory the helper is given. A case that needs a project of its own makes it in OUT/cases/CASE/project; the build.sh
# cases put byte copies of build.sh and package-provenance.rb in it, and a stand-in dotnet and mktemp in
# OUT/cases/CASE/bin. It needs Ruby's standard library, git, bash and mktemp: no dotnet, no network. --only runs just the
# cases it names, in file order; an empty, unknown or repeated name is a usage error (exit 2), refused before anything is
# made. It prints a pass or FAIL line per case, writes OUT/results.json, and exits 0 only if every selected case ran and
# passed.
#
# The release-* cases run retain-release from their case directory, not from the SDK root, against OUT/cases/CASE/mirror,
# a git checkout of the generated SDK whose HEAD is its own commit and not the generator source the receipt names. The
# ordinary retain and release-local modes alone, without the build.sh and path-aliasing cases:
#
#   ruby package-provenance-test.rb OUT --only retains-actual-package-and-same-run-schema,unknown-local-ci-ids-kept-null,release-receipt-retains-from-mirror-checkout,release-receipt-refusals-before-writing
require "digest"
require "fileutils"
require "json"
require "open3"
require "zlib"

HELPER = File.join(__dir__, "package-provenance.rb")
BUILD_SCRIPT = File.join(__dir__, "build.sh")
PACKAGE_VERSION = "2.0.0"
PIPELINE = "4242"
CAPTURE_JOB = "9001"
BUILD_JOB = "9002"
SCHEMA = %({"swagger":"2.0","paths":{}}\n)
CAPTURE_SOURCE = "s3://files-swagger-schema/swagger/swagger.json"
STEPS = [
  [ "format sdk/FilesCom/FilesCom.csproj", 0 ],
  [ "build sdk/FilesCom/FilesCom.csproj", 0 ],
  [ "pack sdk/FilesCom/FilesCom.csproj", 0 ],
  [ "build examples/client-journey/client-journey.csproj", 0 ]
]
# The package's lib/ members, name => [content, ZIP method]: 0 stores a member, 8 deflates it.
LIB = {
  "lib/net10.0/FilesCom.dll" => [ "MZ FilesCom for net10.0\n" * 40, 8 ],
  "lib/net8.0/FilesCom.dll" => [ "MZ FilesCom for net8.0\n", 0 ],
  "lib/netstandard2.0/FilesCom.xml" => [ "<doc>FilesCom for netstandard2.0</doc>\n" * 20, 8 ]
}
CONTENT_TYPES = %(<?xml version="1.0" encoding="utf-8"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="nuspec" ContentType="application/octet" /></Types>\n)
# The generated inputs the manifest must list, and files beside them it must not: one at the top that is not an input,
# and build output under bin/ and obj/. A link under sdk/FilesCom/ is made beside them, which it must not list either.
SOURCE_FILES = {
  "global.json" => %({ "sdk": { "version": "10.0.401" } }\n),
  "sdk/FilesCom/FilesCom.csproj" => %(<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><Version>2.0.0</Version></PropertyGroup></Project>\n),
  "sdk/FilesCom/FilesClient.cs" => "namespace FilesCom { public class FilesClient { } }\n",
  "sdk/FilesCom/Models/User.cs" => "namespace FilesCom.Models { public class User { } }\n"
}
OTHER_FILES = {
  "README.md" => "Not a generated input.\n",
  "sdk/FilesCom/bin/Production/net8.0/FilesCom.dll" => "build output\n",
  "sdk/FilesCom/obj/project.assets.json" => "{}\n"
}
GIT_ENV = { "GIT_DIR" => nil, "GIT_WORK_TREE" => nil, "GIT_INDEX_FILE" => nil, "GIT_CONFIG_NOSYSTEM" => "1" }
RESULTS = []
# The declared cases, [name, body], in file order.
CASES = []
# The SDK commands build.sh runs in a build case's project, in order, as the stand-in dotnet gets its arguments, with
# build.sh's work directory shown as $work. Each is a step of the recipe but VERSION_QUERY, whose output is the version.
VERSION_QUERY = "msbuild sdk/FilesCom/FilesCom.csproj -getProperty:Version"
RECIPE = [
  "format sdk/FilesCom/FilesCom.csproj",
  "format --verify-no-changes sdk/FilesCom/FilesCom.csproj",
  "format sdk/FilesTests/FilesTests.csproj",
  "format --verify-no-changes sdk/FilesTests/FilesTests.csproj",
  "format sdk/FilesContractTests/FilesContractTests.csproj",
  "format --verify-no-changes sdk/FilesContractTests/FilesContractTests.csproj",
  "build sdk/FilesCom/FilesCom.csproj --configuration Production",
  "pack sdk/FilesCom/FilesCom.csproj --configuration Production --no-build --output $work/packages",
  VERSION_QUERY,
  "format examples/demo/demo.csproj",
  "format --verify-no-changes examples/demo/demo.csproj",
  "build examples/demo/demo.csproj --configuration Release"
]
# The command whose exit the stand-in keeps from being recorded: steps are recorded before it, and the pack comes after.
BREAK_COMMAND = "build sdk/FilesCom/FilesCom.csproj --configuration Production"
# The dotnet the build cases put first on build.sh's PATH. It appends its arguments, as one line, to STAND_IN_LOG; packs
# by copying STAND_IN_PACKAGE into --output as FilesCom.STAND_IN_VERSION.nupkg; answers the version query with
# STAND_IN_VERSION; and otherwise succeeds. When its arguments are STAND_IN_BREAK, it breaks the record build.sh is about
# to append this command's exit to (it holds the one $TMPDIR/*/steps.tsv aside and makes a directory in its place), then
# exits STAND_IN_BREAK_EXIT. Any later command puts the record back first, so a build that went on would keep recording,
# and retain would see every exit but the lost one, each 0.
STAND_IN_DOTNET = <<~'RUBY'
  #!/usr/bin/env ruby
  require "fileutils"

  command = ARGV.join(" ")
  File.open(ENV.fetch("STAND_IN_LOG"), "a") { |log| log.puts(command) }
  work_parent = ENV.fetch("TMPDIR")
  Dir.glob("*/steps.tsv.held", base: work_parent).each do |held|
    record = File.join(work_parent, held.delete_suffix(".held"))
    Dir.rmdir(record)
    File.rename(File.join(work_parent, held), record)
  end
  if command == ENV["STAND_IN_BREAK"]
    records = Dir.glob("*/steps.tsv", base: work_parent)
    abort "stand-in dotnet: #{records.size} steps.tsv under #{work_parent}, not one" unless records.size == 1
    record = File.join(work_parent, records.first)
    File.rename(record, "#{record}.held")
    Dir.mkdir(record)
    status = Integer(ENV.fetch("STAND_IN_BREAK_EXIT", "0"))
    warn "stand-in dotnet: build failed" unless status.zero?
    exit status
  end
  case ARGV.first
  when "pack"
    output = ARGV.fetch(ARGV.index("--output") + 1)
    FileUtils.mkdir_p(output)
    FileUtils.cp(ENV.fetch("STAND_IN_PACKAGE"), File.join(output, "FilesCom.#{ENV.fetch("STAND_IN_VERSION")}.nupkg"))
  when "msbuild"
    puts ENV.fetch("STAND_IN_VERSION") if ARGV.include?("-getProperty:Version")
  end
RUBY
# The mktemp the build cases put first on build.sh's PATH. A bare `mktemp -d` need not make its directory in TMPDIR (on
# macOS it was made under /var/folders), so this one has the system's own mktemp, STAND_IN_REAL_MKTEMP, make build.sh's
# work directory in the case's TMPDIR from an explicit template: there the stand-in dotnet finds build.sh's record, and
# the case sees that build.sh removed it. Anything but `-d` is refused.
STAND_IN_MKTEMP = <<~'SH'
  #!/bin/sh
  if [ "$#" -ne 1 ] || [ "$1" != "-d" ]; then
    echo "stand-in mktemp: only -d is expected, not: $*" >&2
    exit 2
  fi
  exec "$STAND_IN_REAL_MKTEMP" -d "$TMPDIR/tmp.XXXXXXXXXX"
SH
USAGE = "usage: package-provenance-test.rb OUT [--only CASE[,CASE...]]"

unless (ARGV.size == 1 || (ARGV.size == 3 && ARGV[1] == "--only")) && !ARGV[0].empty?
  warn USAGE
  exit 2
end
OUT = File.expand_path(ARGV[0])
# The case names --only gives, or nil to run every case. Unknown names are refused once every case is declared.
ONLY = ARGV.size == 3 ? ARGV[2].split(",", -1) : nil
if ONLY && (ONLY.empty? || ONLY.include?(""))
  warn "package-provenance-test: --only #{ARGV[2].inspect} names an empty case; #{USAGE}"
  exit 2
end
repeated = ONLY.to_a.select { |name| ONLY.count(name) > 1 }.uniq
unless repeated.empty?
  warn "package-provenance-test: --only names #{repeated.join(", ")} more than once"
  exit 2
end
if File.exist?(OUT) || File.symlink?(OUT)
  warn "package-provenance-test: #{OUT} exists; it must not, so nothing from an earlier run is taken for this one's"
  exit 2
end

def git(directory, *arguments)
  output, errors, status = Open3.capture3(GIT_ENV, "git", *arguments, chdir: directory)
  raise "git #{arguments.join(" ")} failed: #{errors.strip}" unless status.success?

  output.strip
end

# A throw-away checkout shaped like generated/dotnet when build.sh calls the helper.
def source_checkout(directory)
  SOURCE_FILES.merge(OTHER_FILES).each do |path, text|
    FileUtils.mkdir_p(File.dirname(File.join(directory, path)))
    File.write(File.join(directory, path), text)
  end
  File.symlink("FilesClient.cs", File.join(directory, "sdk/FilesCom/Link.cs"))
  git(directory, "init", "-q")
  git(directory, "add", "-A")
  git(directory, "-c", "user.name=package-provenance-test", "-c", "user.email=package-provenance-test@example.invalid", "-c", "commit.gpgsign=false",
      "-c", "core.hooksPath=/dev/null", "commit", "-q", "--no-verify", "-m", "package-provenance-test fixture")
  { dir: directory, commit: git(directory, "rev-parse", "HEAD"), tree: git(directory, "rev-parse", "HEAD^{tree}") }
end

def deflate(content)
  deflater = Zlib::Deflate.new(Zlib::BEST_COMPRESSION, -Zlib::MAX_WBITS)
  deflater.deflate(content, Zlib::FINISH)
ensure
  deflater&.close
end

# A ZIP archive of MEMBERS, name => [content, method]: each local header and its data, then the central directory and
# its end record.
def zip_archive(members)
  body = "".b
  directory = "".b
  members.each do |name, (content, method)|
    content = content.b
    data = method == 8 ? deflate(content) : content
    crc = Zlib.crc32(content)
    directory << [ 0x02014b50, 20, 20, 0, method, 0, 0x21, crc, data.bytesize, content.bytesize, name.bytesize, 0, 0, 0, 0, 0, body.bytesize ].pack("VvvvvvvVVVvvvvvVV") << name.b
    body << [ 0x04034b50, 20, 0, method, 0, 0x21, crc, data.bytesize, content.bytesize, name.bytesize, 0 ].pack("VvvvvvVVVvv") << name.b << data
  end
  body + directory + [ 0x06054b50, 0, 0, members.size, members.size, directory.bytesize, body.bytesize, 0 ].pack("VvvvvVVv")
end

def nuspec(version)
  <<~XML
    <?xml version="1.0" encoding="utf-8"?>
    <package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
      <metadata>
        <id>FilesCom</id>
        <version>#{version}</version>
        <authors>Files.com</authors>
        <dependencies>
          <group targetFramework="net8.0">
            <dependency id="System.Text.Json" version="8.0.5" exclude="Build,Analyzers" />
          </group>
        </dependencies>
      </metadata>
    </package>
  XML
end

# One case's inputs, made in DIRECTORY. Each option changes one thing from the inputs that are kept.
def inputs(directory, version: PACKAGE_VERSION, nuspec_version: PACKAGE_VERSION, ids: nil, record: {}, steps: STEPS)
  ids ||= capture_ids
  package = File.join(directory, "packages", "FilesCom.#{version}.nupkg")
  FileUtils.mkdir_p(File.dirname(package))
  File.binwrite(package, zip_archive({ "FilesCom.nuspec" => [ nuspec(nuspec_version), 8 ], "[Content_Types].xml" => [ CONTENT_TYPES, 0 ] }.merge(LIB)))
  capture = File.join(directory, "capture", "swagger_doc-production.json")
  FileUtils.mkdir_p(File.dirname(capture))
  File.binwrite(capture, SCHEMA)
  File.write("#{capture}.sha256", "#{Digest::SHA256.hexdigest(SCHEMA)}  #{File.basename(capture)}\n")
  provenance = { "format" => "gollum-schema-capture/1", "flavor" => "public", "source" => CAPTURE_SOURCE, "sha256" => Digest::SHA256.hexdigest(SCHEMA),
                 "bytes" => SCHEMA.bytesize, "captured_at" => Time.now.utc.strftime("%Y-%m-%dT%H:%M:%SZ") }.merge(ids).merge(record)
  File.write("#{capture}.provenance.json", "#{JSON.pretty_generate(provenance)}\n")
  steps_file = File.join(directory, "steps.tsv")
  File.write(steps_file, steps.map { |name, status| "#{name}\t#{status}\n" }.join)
  { package: package, version: version, capture: capture, steps: steps_file, artifact: File.join(directory, "artifact") }
end

# The environment build.sh gives the helper, set in full: none of this process's own CI or git variables reach it.
def helper_env(capture, ci = {})
  { "SWAGGER_CAPTURE" => capture, "CI_PIPELINE_ID" => ci["pipeline_id"], "CI_JOB_ID" => ci["job_id"], "CI_COMMIT_SHA" => ci["commit_sha"],
    "GIT_DIR" => nil, "GIT_WORK_TREE" => nil, "GIT_INDEX_FILE" => nil, "RUBYOPT" => nil, "RUBYLIB" => nil }
end

# The IDs a capture records when it ran in this pipeline, in its own job, at the checkout's commit.
def capture_ids(source = SOURCE)
  { "pipeline_id" => PIPELINE, "job_id" => CAPTURE_JOB, "commit_sha" => source[:commit] }
end

# A build in the capture's pipeline, at the checkout's commit, in a job of its own.
def same_pipeline_ci(source = SOURCE)
  { "pipeline_id" => PIPELINE, "job_id" => BUILD_JOB, "commit_sha" => source[:commit] }
end

def run_helper(env, command, input, project: SOURCE[:dir])
  arguments = command == "check" ? [ input[:artifact] ] : [ input[:artifact], input[:package], input[:version], input[:steps] ]
  stdout, stderr, status = Open3.capture3(env, "ruby", HELPER, command, *arguments, chdir: project)
  { command: command, exit: status.exitstatus, stdout: stdout, stderr: stderr }
end

# The files under DIRECTORY, hidden ones included, by relative path; nil when it does not exist.
def listing(directory)
  return nil unless File.exist?(directory)

  Dir.glob("**/*", File::FNM_DOTMATCH, base: directory).select { |path| File.file?(File.join(directory, path)) }.sort
end

# Every string in a parsed JSON value, keys included, with the key it is the value of.
def string_values(value, key = nil)
  case value
  when Hash then value.flat_map { |k, v| [ [ nil, k ] ] + string_values(v, k) }
  when Array then value.flat_map { |item| string_values(item, key) }
  when String then [ [ key, value ] ]
  else []
  end
end

def same(problems, what, actual, expected)
  problems << "#{what} is #{actual.inspect}, not #{expected.inspect}" unless actual == expected
end

def identical(problems, what, path, bytes)
  return if File.exist?(path) && File.binread(path) == bytes.b

  problems << "#{what} is not byte-identical to its original (SHA-256 #{File.exist?(path) ? Digest::SHA256.file(path).hexdigest : "missing"}, not #{Digest::SHA256.hexdigest(bytes)})"
end

def succeeded?(problems, run)
  return true if run[:exit] == 0

  problems << "#{run[:command]} exited #{run[:exit].inspect}: #{run[:stderr].strip}"
  false
end

# check and then retain must each refuse, with one "package-provenance:" line naming REASON, and leave OUT as it was:
# absent, or holding only KEPT. A refusal only retain can see lets check pass first.
def expect_refusals(problems, env, input, reason, check_passes: false, kept: nil)
  %w[check retain].each do |command|
    run = run_helper(env, command, input)
    if command == "check" && check_passes
      succeeded?(problems, run)
      next
    end
    problems << "#{command} exited #{run[:exit].inspect}, not 1" unless run[:exit] == 1
    lines = run[:stderr].lines
    unless lines.size == 1 && lines.first.start_with?("package-provenance: ") && lines.first.match?(reason)
      problems << "#{command} wrote #{run[:stderr].inspect} to stderr, not one package-provenance: line matching #{reason.inspect}"
    end
    problems << "#{command} left a package-provenance.json" if File.exist?(File.join(input[:artifact], "package-provenance.json"))
    problems << "#{command} left OUT holding #{listing(input[:artifact]).inspect}, not #{kept.inspect}" unless listing(input[:artifact]) == kept
  end
end

# Declares the case NAME, whose BODY is given its directory and the list its problems go in. The cases run once every
# one is declared, in file order: all of them, or those --only names.
def run_case(name, &body)
  raise ArgumentError, "the case #{name} is declared twice" if CASES.assoc(name)

  CASES << [ name, body ]
end

def execute_case(name, body)
  directory = File.join(OUT, "cases", name)
  FileUtils.mkdir_p(directory)
  problems = []
  begin
    raise "the fixture checkout could not be made: #{SOURCE.message}" if SOURCE.is_a?(Exception)

    body.call(directory, problems)
  rescue StandardError => e
    problems << "#{e.class}: #{e.message}"
  end
  RESULTS << { "case" => name, "pass" => problems.empty?, "detail" => problems.empty? ? "as expected" : problems.join("; ") }
  puts(problems.empty? ? "pass #{name}" : "FAIL #{name}: #{problems.join("; ")}")
end

# Every entry under DIRECTORY but .git, hidden ones included, by type and relative path, links not followed.
def project_tree(directory)
  Dir.glob("**/*", File::FNM_DOTMATCH, base: directory)
     .reject { |path| %w[. ..].include?(File.basename(path)) || path == ".git" || path.start_with?(".git/") }
     .sort.map { |path| "#{File.lstat(File.join(directory, path)).ftype} #{path}" }
end

# A generated project build.sh runs in, made in DIRECTORY: the checkout above with one example application, and byte
# copies of build.sh and package-provenance.rb beside them, as the generator writes them into generated/dotnet.
def build_project(directory)
  FileUtils.mkdir_p(File.join(directory, "examples", "demo"))
  File.write(File.join(directory, "examples", "demo", "demo.csproj"), %(<Project Sdk="Microsoft.NET.Sdk"></Project>\n))
  [ BUILD_SCRIPT, HELPER ].each { |original| FileUtils.cp(original, directory) }
  File.chmod(0o755, File.join(directory, "build.sh"))
  source_checkout(directory)
end

# The system's own mktemp, from this process's PATH, which the stand-in mktemp has make build.sh's work directory.
def real_mktemp
  found = ENV["PATH"].to_s.split(File::PATH_SEPARATOR).map { |dir| File.join(dir, "mktemp") }
                     .find { |path| File.file?(path) && File.executable?(path) }
  found or raise "no mktemp on PATH for the stand-in mktemp to use"
end

# What a build case runs build.sh with, made in DIRECTORY: its project; the capture tuple and the package the stand-in
# dotnet packs, made as for the helper's cases; the stand-in dotnet and mktemp; and an empty TMPDIR, where the stand-in
# mktemp has build.sh's work directory made.
def build_fixture(directory, problems)
  project = build_project(File.join(directory, "project"))
  [ BUILD_SCRIPT, HELPER ].each do |original|
    copy = File.join(project[:dir], File.basename(original))
    identical(problems, "the project's copy of #{File.basename(original)}", copy, File.binread(original))
  end
  input = inputs(directory, ids: capture_ids(project))
  bin = File.join(directory, "bin")
  FileUtils.mkdir_p(bin)
  { "dotnet" => STAND_IN_DOTNET, "mktemp" => STAND_IN_MKTEMP }.each do |tool, script|
    File.write(File.join(bin, tool), script)
    File.chmod(0o755, File.join(bin, tool))
  end
  tmp = File.join(directory, "tmp")
  FileUtils.mkdir_p(tmp)
  { project: project, input: input, bin: bin, tmp: tmp, log: File.join(directory, "dotnet.log"), artifact: input[:artifact] }
end

# Runs the project's build.sh as CI does, from the project, with an environment it sets in full: the stand-in dotnet and
# mktemp first on PATH, the case's TMPDIR, the artifact directory ARTIFACT, the capture, and a build in the capture's
# pipeline at HEAD from another job. With BREAK_EXIT, the stand-in keeps BREAK_COMMAND's exit from being recorded and
# exits BREAK_EXIT.
def run_build(fixture, artifact: fixture[:artifact], break_exit: nil)
  ci = same_pipeline_ci(fixture[:project])
  env = {
    "PATH" => [ fixture[:bin], ENV["PATH"] ].compact.reject(&:empty?).join(File::PATH_SEPARATOR),
    "TMPDIR" => fixture[:tmp], "STAND_IN_REAL_MKTEMP" => real_mktemp,
    "FILES_DOTNET_CI_ARTIFACT_DIR" => artifact,
    "SWAGGER_CAPTURE" => fixture[:input][:capture],
    "CI_PIPELINE_ID" => ci["pipeline_id"], "CI_JOB_ID" => ci["job_id"], "CI_COMMIT_SHA" => ci["commit_sha"],
    "STAND_IN_LOG" => fixture[:log], "STAND_IN_PACKAGE" => fixture[:input][:package], "STAND_IN_VERSION" => PACKAGE_VERSION,
    "STAND_IN_BREAK" => break_exit && BREAK_COMMAND, "STAND_IN_BREAK_EXIT" => break_exit&.to_s,
    "GIT_DIR" => nil, "GIT_WORK_TREE" => nil, "GIT_INDEX_FILE" => nil, "RUBYOPT" => nil, "RUBYLIB" => nil, "BASH_ENV" => nil, "ENV" => nil
  }
  stdout, stderr, status = Open3.capture3(env, "bash", File.join(fixture[:project][:dir], "build.sh"), chdir: fixture[:project][:dir])
  { command: "build.sh", exit: status.exitstatus, stdout: stdout, stderr: stderr }
end

# The commands the stand-in dotnet ran, in order, with build.sh's work directory under TMPDIR shown as $work.
def stand_in_commands(fixture)
  return [] unless File.exist?(fixture[:log])

  File.read(fixture[:log]).lines.map { |line| line.chomp.gsub(%r{#{Regexp.escape(fixture[:tmp])}/[^/\s]+}, "$work") }
end

def exited_one(problems, run)
  problems << "#{run[:command]} exited #{run[:exit].inspect}, not 1 (stderr #{run[:stderr].strip.inspect})" unless run[:exit] == 1
end

# A build that stopped keeps nothing: its artifact directory is absent or empty.
def nothing_kept(problems, fixture)
  artifact = fixture[:artifact]
  return if !File.exist?(artifact) || (File.directory?(artifact) && Dir.empty?(artifact))

  problems << "the artifact directory holds #{(listing(artifact) || []).inspect}"
end

# build.sh removes its work directory however it ends, and makes it only after check.
def no_work_left(problems, fixture)
  left = Dir.children(fixture[:tmp])
  problems << "build.sh left #{left.sort.inspect} in its TMPDIR" unless left.empty?
end

# The whole contract: the package build.sh packed and the schema this run captured are kept byte for byte, and the
# provenance accounts for them, for the source and for this build, which joins the capture's pipeline from another job.
run_case("retains-actual-package-and-same-run-schema") do |directory, problems|
  input = inputs(directory)
  ci = same_pipeline_ci
  env = helper_env(input[:capture], ci)
  next unless succeeded?(problems, run_helper(env, "check", input))

  problems << "check wrote #{listing(input[:artifact]).inspect} into OUT" unless listing(input[:artifact]).nil?
  next unless succeeded?(problems, run_helper(env, "retain", input))

  artifact = input[:artifact]
  name = File.basename(input[:capture])
  package_name = "FilesCom.#{PACKAGE_VERSION}.nupkg"
  package = File.binread(input[:package])
  sums = File.binread("#{input[:capture]}.sha256")
  record = File.binread("#{input[:capture]}.provenance.json")
  manifest = SOURCE_FILES.keys.sort.map { |path| "#{Digest::SHA256.hexdigest(SOURCE_FILES[path])}  #{path}\n" }.join
  text = File.read(File.join(artifact, "package-provenance.json"))
  provenance = JSON.parse(text)

  same(problems, "OUT", listing(artifact),
       [ package_name, "generated-input.sha256", "package-provenance.json", "schema/#{name}", "schema/#{name}.sha256", "schema/#{name}.provenance.json" ].sort)
  identical(problems, package_name, File.join(artifact, package_name), package)
  identical(problems, "schema/#{name}", File.join(artifact, "schema", name), SCHEMA)
  identical(problems, "schema/#{name}.sha256", File.join(artifact, "schema", "#{name}.sha256"), sums)
  identical(problems, "schema/#{name}.provenance.json", File.join(artifact, "schema", "#{name}.provenance.json"), record)
  identical(problems, "generated-input.sha256", File.join(artifact, "generated-input.sha256"), manifest)
  same(problems, "format", provenance["format"], "filescom-dotnet-package-provenance/1")
  problems << "at #{provenance["at"].inspect} is not a UTC time to the second" unless provenance["at"].to_s.match?(/\A\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z\z/)
  same(problems, "package id and version", provenance["package"]&.values_at("id", "version"), [ "FilesCom", PACKAGE_VERSION ])
  same(problems, "package archive", provenance.dig("package", "archive"),
       { "path" => package_name, "sha256" => Digest::SHA256.hexdigest(package), "bytes" => package.bytesize,
         "sha512_hex" => Digest::SHA512.hexdigest(package), "sha512_base64" => [ Digest::SHA512.digest(package) ].pack("m0") })
  same(problems, "lib_members, in order", provenance.dig("package", "lib_members").to_a, LIB.sort.map { |path, (content, _)| [ path, Digest::SHA256.hexdigest(content) ] })
  same(problems, "target_frameworks", provenance.dig("package", "target_frameworks"), [ "net10.0", "net8.0", "netstandard2.0" ])
  same(problems, "source", provenance["source"], { "commit" => SOURCE[:commit], "tree" => SOURCE[:tree] })
  same(problems, "schema", provenance["schema"],
       { "path" => "schema/#{name}", "sha256" => Digest::SHA256.hexdigest(SCHEMA), "bytes" => SCHEMA.bytesize,
         "capture_provenance" => JSON.parse(record), "capture_provenance_sha256" => Digest::SHA256.hexdigest(record),
         "capture_sha256_file" => { "path" => "schema/#{name}.sha256", "sha256" => Digest::SHA256.hexdigest(sums) } })
  same(problems, "generated_input", provenance["generated_input"],
       { "manifest" => { "path" => "generated-input.sha256", "sha256" => Digest::SHA256.hexdigest(manifest), "bytes" => manifest.bytesize, "files" => SOURCE_FILES.size } })
  same(problems, "build", provenance["build"], { "commands" => STEPS.map { |step, status| { "name" => step, "exit" => status } }, "ci" => ci })
  same(problems, "join", provenance["join"], { "pipeline" => "same", "source_commit" => "same", "schema_sha256" => "same" })
  # Every path names a kept file relative to the provenance's directory, and none is absolute.
  strings = string_values(provenance)
  unresolved = strings.select { |key, _| key == "path" }.map(&:last).reject { |path| File.file?(File.join(artifact, path)) }
  problems << "paths that name no file beside the provenance: #{unresolved.inspect}" if unresolved.any?
  absolute = strings.reject { |key, _| key == "sha512_base64" }.map(&:last).select { |value| value.start_with?("/") }
  absolute << OUT if text.include?(OUT) || text.include?(File.realpath(OUT))
  problems << "absolute paths in the provenance: #{absolute.inspect}" if absolute.any?
end

# Off CI, or with a capture that recorded none, nothing is known to join: the IDs stay null and the joins unknown.
run_case("unknown-local-ci-ids-kept-null") do |directory, problems|
  input = inputs(directory, ids: { "pipeline_id" => nil, "job_id" => nil, "commit_sha" => nil })
  env = helper_env(input[:capture])
  next unless succeeded?(problems, run_helper(env, "check", input)) && succeeded?(problems, run_helper(env, "retain", input))

  provenance = JSON.parse(File.read(File.join(input[:artifact], "package-provenance.json")))
  same(problems, "build.ci", provenance.dig("build", "ci"), { "pipeline_id" => nil, "job_id" => nil, "commit_sha" => nil })
  same(problems, "join", provenance["join"], { "pipeline" => "unknown", "source_commit" => "unknown", "schema_sha256" => "same" })
  same(problems, "the capture's IDs", provenance.dig("schema", "capture_provenance")&.values_at("pipeline_id", "job_id", "commit_sha"), [ nil, nil, nil ])
end

# Capture bytes that no longer match their .sha256 are not the schema this run captured.
run_case("refuses-capture-sha-mismatch") do |directory, problems|
  input = inputs(directory)
  File.binwrite(input[:capture], "#{SCHEMA}\n")
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, /\.sha256 lists SHA-256 \h{64}, not the capture's/)
end

# The internal schema never stands in for the public one the package is generated from.
run_case("refuses-internal-or-other-source-capture") do |directory, problems|
  input = inputs(directory, record: { "flavor" => "internal" })
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, /has flavor "internal"/)
end

# A capture provenance that is not exactly capture-schema.sh's is not trusted in part.
run_case("refuses-capture-provenance-extra-key") do |directory, problems|
  input = inputs(directory, record: { "label" => "public2a" })
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, /has keys .*\blabel\b.*, not exactly/)
end

# A capture from another pipeline is not this run's schema.
run_case("refuses-pipeline-mismatch") do |directory, problems|
  input = inputs(directory)
  ci = same_pipeline_ci.merge("pipeline_id" => "4243")
  expect_refusals(problems, helper_env(input[:capture], ci), input, /captured in pipeline 4242, not this build's pipeline 4243/)
end

# The commit CI reports for this build must be the one checked out, or the source it names is not the one packed.
run_case("refuses-commit-mismatch") do |directory, problems|
  input = inputs(directory)
  ci = same_pipeline_ci.merge("commit_sha" => Digest::SHA1.hexdigest("another commit"))
  expect_refusals(problems, helper_env(input[:capture], ci), input, /CI_COMMIT_SHA \h+ is not the source commit/)
end

# The version kept is the one inside the package, not only the one in its file name.
run_case("refuses-version-not-in-package") do |directory, problems|
  input = inputs(directory, version: "2.0.1")
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, /FilesCom\.nuspec has version 2\.0\.0, not 2\.0\.1/, check_passes: true)
end

# A recipe with a failed step vouches for no package.
run_case("refuses-failed-step") do |directory, problems|
  input = inputs(directory, steps: STEPS.map { |step, status| [ step, step.start_with?("pack ") ? 1 : status ] })
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, %r{pack sdk/FilesCom/FilesCom\.csproj exited 1}, check_passes: true)
end

# A provenance could be read as covering whatever OUT already held, so OUT must start empty; what is there stays.
run_case("refuses-nonempty-artifact-dir") do |directory, problems|
  input = inputs(directory)
  FileUtils.mkdir_p(input[:artifact])
  File.write(File.join(input[:artifact], "keep.txt"), "kept\n")
  expect_refusals(problems, helper_env(input[:capture], same_pipeline_ci), input, /is not an empty directory/, kept: [ "keep.txt" ])
  same(problems, "keep.txt", File.read(File.join(input[:artifact], "keep.txt")), "kept\n")
end

# Without the same-run capture there is no schema to keep with the package.
run_case("refuses-missing-capture") do |directory, problems|
  input = inputs(directory)
  expect_refusals(problems, helper_env(nil, same_pipeline_ci), input, /SWAGGER_CAPTURE is not set/)
end

# The generator source a release receipt names: object names of no repository here, unlike the mirror checkout's HEAD.
RELEASE_SOURCE = { "commit" => Digest::SHA1.hexdigest("generator source commit"), "tree" => Digest::SHA1.hexdigest("generator source tree") }
RELEASE_COMMANDS = [ { "name" => "dotnet pack sdk/FilesCom/FilesCom.csproj --configuration Production", "exit" => 0 } ]
# The release job's build, in the capture's pipeline from a job of its own, at the generator source.
RELEASE_CI = { "pipeline_id" => PIPELINE, "job_id" => "9003", "commit_sha" => RELEASE_SOURCE["commit"] }

# A release-local pack's inputs, made in DIRECTORY: the mirror checkout it packed in, the package, the capture taken at
# the generator source, and the receipt the release job writes of that pack, kept as a Hash to write with write_receipt.
def release_inputs(directory)
  mirror = source_checkout(File.join(directory, "mirror"))
  input = inputs(directory, ids: { "pipeline_id" => PIPELINE, "job_id" => CAPTURE_JOB, "commit_sha" => RELEASE_SOURCE["commit"] })
  package = File.binread(input[:package])
  manifest = SOURCE_FILES.keys.sort.map { |path| "#{Digest::SHA256.hexdigest(SOURCE_FILES[path])}  #{path}\n" }.join
  receipt = {
    "format" => "filescom-dotnet-release-pack-receipt/1",
    "source" => RELEASE_SOURCE.dup,
    "build" => { "commands" => RELEASE_COMMANDS.map(&:dup), "ci" => RELEASE_CI.dup },
    "package" => { "id" => "FilesCom", "version" => PACKAGE_VERSION, "sha256" => Digest::SHA256.hexdigest(package), "bytes" => package.bytesize },
    "generated_input" => { "manifest" => { "sha256" => Digest::SHA256.hexdigest(manifest), "bytes" => manifest.bytesize, "files" => SOURCE_FILES.size } },
    "mirror" => { "commit" => mirror[:commit], "tag" => "v#{PACKAGE_VERSION}" }
  }
  input.merge(mirror: mirror, manifest: manifest, receipt: receipt, receipt_path: File.join(directory, "pack-receipt.json"))
end

def write_receipt(input)
  File.write(input[:receipt_path], "#{JSON.pretty_generate(input[:receipt])}\n")
end

# Runs retain-release as a release job does, from DIRECTORY: neither the SDK root nor a git checkout.
def run_release(env, input, directory)
  arguments = [ input[:artifact], input[:package], input[:version], input[:mirror][:dir], input[:receipt_path] ]
  stdout, stderr, status = Open3.capture3(env, "ruby", HELPER, "retain-release", *arguments, chdir: directory)
  { command: "retain-release", exit: status.exitstatus, stdout: stdout, stderr: stderr }
end

# A release-local pack in a mirror checkout is kept as its receipt records it: the source is the receipt's generator
# source, not the mirror's HEAD, the build is the receipt's, not the release job's own CI variables, the manifest is of
# the SDK root it names, and the provenance, in the ordinary format, names this mode and claims neither build.sh's recipe
# nor publication.
run_case("release-receipt-retains-from-mirror-checkout") do |directory, problems|
  input = release_inputs(directory)
  write_receipt(input)
  # Variables that would refuse an ordinary retain here (another pipeline, the mirror's commit): retain-release reads none.
  env = helper_env(input[:capture], { "pipeline_id" => "4243", "job_id" => BUILD_JOB, "commit_sha" => input[:mirror][:commit] })
  next unless succeeded?(problems, run_release(env, input, directory))

  artifact = input[:artifact]
  name = File.basename(input[:capture])
  package_name = "FilesCom.#{PACKAGE_VERSION}.nupkg"
  package = File.binread(input[:package])
  receipt = File.binread(input[:receipt_path])
  text = File.read(File.join(artifact, "package-provenance.json"))
  provenance = JSON.parse(text)

  same(problems, "OUT", listing(artifact),
       [ package_name, "generated-input.sha256", "package-provenance.json", "schema/#{name}", "schema/#{name}.sha256", "schema/#{name}.provenance.json" ].sort)
  identical(problems, package_name, File.join(artifact, package_name), package)
  identical(problems, "generated-input.sha256", File.join(artifact, "generated-input.sha256"), input[:manifest])
  same(problems, "format", provenance["format"], "filescom-dotnet-package-provenance/1")
  problems << "producer #{provenance["producer"].inspect} does not name retain-release" unless provenance["producer"].to_s.include?("retain-release")
  same(problems, "source", provenance["source"], RELEASE_SOURCE)
  same(problems, "package archive SHA-256 and bytes", provenance.dig("package", "archive")&.values_at("sha256", "bytes"), [ Digest::SHA256.hexdigest(package), package.bytesize ])
  same(problems, "target_frameworks", provenance.dig("package", "target_frameworks"), [ "net10.0", "net8.0", "netstandard2.0" ])
  same(problems, "generated_input", provenance["generated_input"],
       { "manifest" => { "path" => "generated-input.sha256", "sha256" => Digest::SHA256.hexdigest(input[:manifest]), "bytes" => input[:manifest].bytesize, "files" => SOURCE_FILES.size } })
  same(problems, "build", provenance["build"], { "commands" => RELEASE_COMMANDS, "ci" => RELEASE_CI })
  same(problems, "join", provenance["join"], { "pipeline" => "same", "source_commit" => "same", "schema_sha256" => "same" })
  same(problems, "release_pack", provenance["release_pack"],
       { "receipt" => { "format" => "filescom-dotnet-release-pack-receipt/1", "sha256" => Digest::SHA256.hexdigest(receipt), "bytes" => receipt.bytesize },
         "mirror" => { "commit" => input[:mirror][:commit], "tag" => "v#{PACKAGE_VERSION}" } })
  disclaimed = provenance["not_claimed"].to_a.join("\n")
  problems << "not_claimed #{provenance["not_claimed"].inspect} does not disclaim build.sh's recipe and publication" unless disclaimed.include?("build.sh") && disclaimed.include?("publication")
  absolute = string_values(provenance).reject { |key, _| key == "sha512_base64" }.map(&:last).select { |value| value.start_with?("/") }
  absolute << OUT if text.include?(OUT) || text.include?(File.realpath(OUT))
  problems << "absolute paths in the provenance: #{absolute.inspect}" if absolute.any?
end

# A receipt that is not of the pack it is given refuses before anything is written: another package's digest, a source
# the capture was not taken at, an SDK root changed since its manifest was recorded, or a command that failed.
run_case("release-receipt-refusals-before-writing") do |directory, problems|
  other = Digest::SHA1.hexdigest("another generator commit")
  variants = {
    "package" => [ ->(input) { input[:receipt]["package"]["sha256"] = Digest::SHA256.hexdigest("another package") },
                   /PACK_RECEIPT records package SHA-256 "\h{64}" and \d+ bytes, not PACKAGE's/ ],
    "source" => [ ->(input) { input[:receipt]["source"]["commit"] = other },
                  /the schema was captured at commit \h+, not the source commit #{other}/ ],
    "manifest" => [ ->(input) { File.write(File.join(input[:mirror][:dir], "sdk/FilesCom/FilesClient.cs"), "changed after the pack\n") },
                    /PACK_RECEIPT records a generated-input manifest of SHA-256 "\h{64}", .* not SDK_ROOT's/ ],
    "failed-command" => [ ->(input) { input[:receipt]["build"]["commands"] << { "name" => "dotnet pack again", "exit" => 1 } },
                          /dotnet pack again exited 1, so the pack did not succeed/ ]
  }
  variants.each do |variant, (change, reason)|
    case_directory = File.join(directory, variant)
    input = release_inputs(case_directory)
    change.call(input)
    write_receipt(input)
    run = run_release(helper_env(input[:capture]), input, case_directory)
    problems << "#{variant}: exited #{run[:exit].inspect}, not 1" unless run[:exit] == 1
    lines = run[:stderr].lines
    unless lines.size == 1 && lines.first.start_with?("package-provenance: ") && lines.first.match?(reason)
      problems << "#{variant}: wrote #{run[:stderr].inspect} to stderr, not one package-provenance: line matching #{reason.inspect}"
    end
    problems << "#{variant}: left #{listing(input[:artifact]).inspect} where OUT was to be" unless listing(input[:artifact]).nil?
  end
end

# A path that reaches the generated project through a link, a "." or ".." component, or the working directory would put
# the artifact inside the SDK CI publishes, so check and retain each refuse it before writing anything; an absolute path
# outside the project, reached through a link, is still taken.
run_case("refuses-output-aliasing-into-project") do |directory, problems|
  project = source_checkout(File.join(directory, "project"))
  root = File.realpath(project[:dir])
  input = inputs(directory, ids: capture_ids(project))
  env = helper_env(input[:capture], same_pipeline_ci(project))
  inside = File.join(project[:dir], "alias-target")
  outside = File.join(directory, "outside")
  [ inside, outside ].each { |path| Dir.mkdir(path) }
  { "inside-link" => inside, "project-link" => project[:dir], "sdk-link" => File.join(project[:dir], "sdk"), "outside-link" => outside }.each do |link, target|
    File.symlink(target, File.join(directory, link))
  end

  accepted = run_helper(env, "check", input.merge(artifact: File.join(directory, "outside-link", "artifact")), project: project[:dir])
  succeeded?(problems, accepted)
  problems << "check wrote #{Dir.children(outside).sort.inspect} into #{outside}" unless Dir.empty?(outside)

  inside_reason = /is inside the generated project #{Regexp.escape(root)}; the package is kept only outside it/
  variants = {
    "a link to a directory in the project" => [ File.join(directory, "inside-link"), inside_reason ],
    "a new directory under a link to the project" => [ File.join(directory, "project-link", "new-artifact"), inside_reason ],
    "the project's own path with a . component" => [ File.join(project[:dir], ".", "new-artifact"), inside_reason ],
    "a relative path" => [ "artifact", /is not an absolute path; the package is kept only at an absolute path outside the generated project/ ],
    # Spelled outside the project, but .. leaves the link's target, project/sdk, so the system resolves it inside.
    "a .. component after a link into the project" => [ File.join(directory, "sdk-link", "..", "new-artifact"),
                                                         /names a parent directory \(\.\.\); give the artifact directory without one/ ]
  }
  before = project_tree(project[:dir])
  variants.each do |variant, (out, reason)|
    %w[check retain].each do |command|
      run = run_helper(env, command, input.merge(artifact: out), project: project[:dir])
      label = "#{command} of #{variant} (#{out})"
      problems << "#{label} exited #{run[:exit].inspect}, not 1" unless run[:exit] == 1
      lines = run[:stderr].lines
      unless lines.size == 1 && lines.first.start_with?("package-provenance: ") && lines.first.match?(reason)
        problems << "#{label} wrote #{run[:stderr].inspect} to stderr, not one package-provenance: line matching #{reason.inspect}"
      end
      written = Dir.glob("**/*package-provenance.json*", File::FNM_DOTMATCH, base: directory)
      problems << "#{label} left #{written.inspect}" unless written.empty?
      after = project_tree(project[:dir])
      problems << "#{label} changed the project: added #{(after - before).inspect}, removed #{(before - after).inspect}" unless after == before
    end
  end
end

# The harness's control: with every command succeeding and recorded, the real build.sh keeps the package it packed, with
# the recipe's commands in build.sh's order, and removes its work directory.
run_case("build-stand-in-recipe-retains") do |directory, problems|
  fixture = build_fixture(directory, problems)
  next unless succeeded?(problems, run_build(fixture))

  package_name = "FilesCom.#{PACKAGE_VERSION}.nupkg"
  identical(problems, "the kept #{package_name}", File.join(fixture[:artifact], package_name), File.binread(fixture[:input][:package]))
  provenance = JSON.parse(File.read(File.join(fixture[:artifact], "package-provenance.json")))
  commands = RECIPE.reject { |command| command == VERSION_QUERY }.map { |command| { "name" => "dotnet #{command}", "exit" => 0 } }
  same(problems, "build.commands", provenance.dig("build", "commands"), commands)
  same(problems, "the stand-in dotnet's commands", stand_in_commands(fixture), RECIPE)
  no_work_left(problems, fixture)
end

# A command that succeeded but whose exit could not be recorded must stop the build, or retain would vouch for a record
# missing a command that ran.
run_case("build-record-failure-after-success-stops") do |directory, problems|
  fixture = build_fixture(directory, problems)
  run = run_build(fixture, break_exit: 0)
  exited_one(problems, run)
  notice = /\AThe exit of "dotnet #{Regexp.escape(BREAK_COMMAND)}" could not be recorded in .*steps\.tsv; the build stops\.$/
  notices = run[:stderr].lines.grep(/could not be recorded/)
  problems << "build.sh wrote #{run[:stderr].inspect} to stderr, not one line matching #{notice.inspect}" unless notices.size == 1 && notices.first.match?(notice)
  same(problems, "the stand-in dotnet's commands", stand_in_commands(fixture), RECIPE.take(RECIPE.index(BREAK_COMMAND) + 1))
  nothing_kept(problems, fixture)
  no_work_left(problems, fixture)
end

# When the command itself failed, its own failure stays the one reported, though its exit could not be recorded either.
run_case("build-command-failure-stays-first-with-record-failure") do |directory, problems|
  fixture = build_fixture(directory, problems)
  run = run_build(fixture, break_exit: 3)
  exited_one(problems, run)
  problems << "stderr #{run[:stderr].inspect} does not hold the failed command's own line" unless run[:stderr].include?("stand-in dotnet: build failed")
  problems << "stderr #{run[:stderr].inspect} reports the record failure as well as the command's own" if run[:stderr].include?("could not be recorded")
  same(problems, "the stand-in dotnet's commands", stand_in_commands(fixture), RECIPE.take(RECIPE.index(BREAK_COMMAND) + 1))
  nothing_kept(problems, fixture)
  no_work_left(problems, fixture)
end

# The helper's check is build.sh's one guard on the artifact directory: a new directory that a link to the project puts
# inside it, though spelled outside, stops the build before any SDK command runs and before the work directory is made.
run_case("build-refuses-alias-output-before-sdk-commands") do |directory, problems|
  fixture = build_fixture(directory, problems)
  File.symlink(fixture[:project][:dir], File.join(directory, "project-link"))
  artifact = File.join(directory, "project-link", "new-artifact")
  before = project_tree(fixture[:project][:dir])
  run = run_build(fixture, artifact: artifact)
  exited_one(problems, run)
  problems << "stderr #{run[:stderr].inspect} does not say the artifact directory is inside the generated project" unless run[:stderr].include?("is inside the generated project")
  same(problems, "the stand-in dotnet's commands", stand_in_commands(fixture), [])
  after = project_tree(fixture[:project][:dir])
  problems << "build.sh changed the project: added #{(after - before).inspect}, removed #{(before - after).inspect}" unless after == before
  no_work_left(problems, fixture)
end

names = CASES.map(&:first)
unknown = ONLY.to_a - names
unless unknown.empty?
  warn "package-provenance-test: --only names no case #{unknown.join(", ")}; the cases are #{names.join(", ")}"
  exit 2
end
SELECTED = ONLY ? names & ONLY : names

FileUtils.mkdir_p(OUT)
SOURCE = begin
  source_checkout(File.join(OUT, "source"))
rescue StandardError => e
  e
end
CASES.each { |name, body| execute_case(name, body) if SELECTED.include?(name) }

passed = RESULTS.count { |result| result["pass"] }
summary = { "selected" => SELECTED, "cases" => RESULTS, "count" => RESULTS.size, "passed" => passed, "failed" => RESULTS.size - passed,
            "status" => RESULTS.size == SELECTED.size && passed == RESULTS.size ? "pass" : "fail" }
File.write(File.join(OUT, "results.json"), "#{JSON.pretty_generate(summary)}\n")
puts "package-provenance-test: #{summary["status"]}, #{passed} of #{SELECTED.size} selected cases passed; #{File.join(OUT, "results.json")}"
exit(summary["status"] == "pass" ? 0 : 1)
