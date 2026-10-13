#!/usr/bin/env ruby
# Keeps the FilesCom package build.sh has just packed, with an account of what it was built from, for CI to keep as an
# artifact. build.sh runs it only when FILES_DOTNET_CI_ARTIFACT_DIR is set, from its own directory (generated/dotnet,
# inside the generator's git checkout):
#
#   ruby package-provenance.rb check OUT
#   ruby package-provenance.rb retain OUT PACKAGE VERSION STEPS
#
# check runs first, before anything is formatted or built, so a build whose package could not be kept fails before it is
# spent. It writes nothing. retain runs last, once every step of the recipe has succeeded: it repeats every check of
# check, then checks the package and the recipe, and keeps them.
#
# Inputs. OUT is the artifact directory. PACKAGE is the archive dotnet pack wrote, named FilesCom.VERSION.nupkg, and
# VERSION the project's MSBuild Version. STEPS is the recipe's record: one "NAME<TAB>EXIT" line per command, in the order
# they ran. SWAGGER_CAPTURE names the public schema this run generated from, as .gitlab-ci/capture-schema.sh captured it,
# beside SWAGGER_CAPTURE.sha256 and SWAGGER_CAPTURE.provenance.json. The working directory's git HEAD is the source
# commit. CI_PIPELINE_ID, CI_JOB_ID and CI_COMMIT_SHA identify this build; unset or empty, each is unknown and recorded
# as null, never guessed. No other environment variable is read.
#
# Outputs (retain only), in OUT:
# - FilesCom.VERSION.nupkg, a byte copy of PACKAGE;
# - schema/NAME, schema/NAME.sha256 and schema/NAME.provenance.json, byte copies of the capture tuple, so the .sha256
#   still checks beside the capture;
# - generated-input.sha256, "SHA256  PATH" lines, sorted by path, for every generated input that was packed: each
#   regular file (not a link) under sdk/FilesCom/ outside any bin/ or obj/ directory, and whichever of
#   Directory.Build.props, NuGet.Config, global.json and _VERSION exist in the working directory, read when retain runs;
# - package-provenance.json, written last: the package's digests (NuGet's SHA-512 among them), its target frameworks
#   and the SHA-256 of each lib/ member; the source commit and tree; the capture, its whole provenance and the digests of
#   its sibling files; the manifest; the recipe's commands and this build's CI identity; and how the capture, the build
#   and the source join. It names every file by a path relative to its own directory, and holds no absolute path.
#
# Refusals. Each prints one line, "package-provenance: REASON", on stderr and exits 1 (a usage error exits 2):
# - OUT is not an absolute path, names a parent directory (..), or lies inside the generated project the working directory
#   is (the SDK CI keeps and publishes), once every link in its existing part is followed; this is checked first;
# - OUT exists and is not an empty directory;
# - SWAGGER_CAPTURE is unset or not a non-empty regular file; its .sha256 is not one line listing its file name (a
#   leading * allowed) under the lowercase SHA-256 of its bytes; its .provenance.json is not a JSON object with exactly
#   the keys format, flavor, source, sha256, bytes, captured_at, pipeline_id, job_id and commit_sha, with format
#   gollum-schema-capture/1, flavor public, source s3://files-swagger-schema/swagger/swagger.json, the capture's SHA-256
#   and size (an integer), captured_at a UTC time to the second, and each ID a string or null;
# - the working directory is not a git checkout with a commit;
# - the capture's pipeline_id and CI_PIPELINE_ID are both known and differ, the capture's commit_sha is known and is not
#   HEAD, or CI_COMMIT_SHA is known and is not HEAD. Job IDs are never compared: the capture ran in a job of its own.
# and, for retain:
# - VERSION is not a package version, or PACKAGE is not a regular file named FilesCom.VERSION.nupkg;
# - PACKAGE is not a ZIP archive this script reads without guessing: one that is ZIP64, spans disks, is encrypted, uses a
#   method other than stored or deflated, repeats a member, or has a member whose local header, size or CRC-32 disagrees
#   with its central directory entry;
# - it has no root FilesCom.nuspec, its nuspec's id is not FilesCom or its version not VERSION, or it has no
#   lib/FRAMEWORK/ member;
# - STEPS is empty, or a line is not NAME<TAB>EXIT, or an EXIT is not 0;
# - sdk/FilesCom/ is missing or holds no file, or a generated input's path has a line break;
# - the provenance would hold an absolute path (a step NAME that names one, say);
# - a copy written to OUT does not hash as the bytes that were checked.
#
# Every refusal but the last comes before anything is written: OUT is left as it was, absent or empty. The last leaves the
# copies already written and removes nothing. package-provenance.json is written last, to a temporary file in OUT then
# renamed into place, so a run that did not finish never leaves one: its presence means every check above held.
#
# Release-local pack. A release job that packs the package itself, with dotnet pack in a generated SDK root that may be a
# mirror checkout whose git HEAD is not the generator source, keeps it with:
#
#   ruby package-provenance.rb retain-release OUT PACKAGE VERSION SDK_ROOT PACK_RECEIPT
#
# It runs from any directory. SDK_ROOT is the generated dotnet root it packed, after its version was stamped: it takes
# the place of the working directory, so OUT must lie outside it, and the manifest is of its inputs. PACK_RECEIPT is the
# release job's own record of that pack, a UTF-8 JSON object with exactly these keys (mirror optional):
#   format           "filescom-dotnet-release-pack-receipt/1"
#   source           {commit, tree}: the generator source it was generated from, each a git object name
#   build            {commands: [{name, exit}, ...], ci: {pipeline_id, job_id, commit_sha}}: its commands in the order
#                    they ran, each named by non-empty text on one line and each exiting 0, and its CI identity, each
#                    ID a string or null
#   package          {id, version, sha256, bytes}: FilesCom, VERSION, and the SHA-256 and size of PACKAGE's bytes
#   generated_input  {manifest: {sha256, bytes, files}}: the manifest of SDK_ROOT's inputs, as below
#   mirror           {commit, tag}: the mirror checkout's commit (a git object name) and intended tag (a string or
#                    null), recorded apart from the source, never as proof of publication
# Everything else is retain's: the same SWAGGER_CAPTURE and its checks, the same package, manifest, OUT and absolute-path
# checks and the same outputs, in the same format. The source and the build's CI identity are the receipt's, not
# SDK_ROOT's git HEAD nor CI_PIPELINE_ID, CI_JOB_ID and CI_COMMIT_SHA, which it does not read; they join the capture as
# retain's do, job IDs never compared. It also refuses, before anything is written, a receipt that is not a regular file
# of that shape, a failed command, and a package or manifest that is not the one the receipt records. The provenance names
# this mode as its producer, adds release_pack (the receipt's format, SHA-256 and size, and the mirror or null), and does
# not claim build.sh's recipe, publication, or that the source names objects of a repository it checked.
require "digest"
require "fileutils"
require "json"
require "open3"
require "zlib"

FORMAT = "filescom-dotnet-package-provenance/1"
PRODUCER = "targets/dotnet/build.sh with package-provenance.rb, after every step of its recipe succeeded"
NOT_CLAIMED = [
  "publication or release of this package",
  "source or schema metadata embedded in the package or its DLLs",
  "a build other than this recipe's"
]
RELEASE_PRODUCER = "package-provenance.rb retain-release, after a release-local dotnet pack that its PACK_RECEIPT records; not targets/dotnet/build.sh"
RELEASE_NOT_CLAIMED = [
  "publication or release of this package, or of the mirror commit or tag",
  "source or schema metadata embedded in the package or its DLLs",
  "targets/dotnet/build.sh's recipe, or the formatting, builds and examples it checks",
  "that the source commit and tree name objects of a git repository: they are the receipt's, checked for their form and against the capture's and build's commits",
  "a build other than the one the receipt records"
]
RECEIPT_FORMAT = "filescom-dotnet-release-pack-receipt/1"
CAPTURE_KEYS = %w[format flavor source sha256 bytes captured_at pipeline_id job_id commit_sha]
CAPTURE_FORMAT = "gollum-schema-capture/1"
CAPTURE_SOURCE = "s3://files-swagger-schema/swagger/swagger.json"
UTC_SECOND = /\A\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z\z/
SHA256_LINE = /\A([0-9a-f]{64}) [ *]([^\n]+)\n?\z/
GIT_OBJECT = /\A(?:[0-9a-f]{40}|[0-9a-f]{64})\z/
# A version as it appears in a package's file name: nothing that leaves the name, such as a slash or a space.
PACKAGE_VERSION = /\A[0-9A-Za-z][0-9A-Za-z.+-]*\z/
# The generated project that is packed, and the generated files beside it that shape the pack.
PROJECT = "sdk/FilesCom"
TOP_INPUTS = %w[Directory.Build.props NuGet.Config global.json _VERSION]
# A slash that starts a string or follows a space, quote or equals sign: the start of an absolute path.
ABSOLUTE_PATH = %r{(?:\A|[\s="'])/}
LOCAL_HEADER = [ 0x04034b50 ].pack("V")
CENTRAL_HEADER = [ 0x02014b50 ].pack("V")
END_RECORD = [ 0x06054b50 ].pack("V")
ZIP64_LOCATOR = [ 0x07064b50 ].pack("V")
# Traditional encryption (bit 0), strong encryption (bit 6), an encrypted central directory (bit 13).
ENCRYPTED = 0x2041
# Bit 3: the CRC-32 and sizes follow the member's data, so its local header may hold zeros; the central directory has them.
DATA_DESCRIPTOR = 0x0008
USAGE = "usage: package-provenance.rb check OUT | package-provenance.rb retain OUT PACKAGE VERSION STEPS | " \
        "package-provenance.rb retain-release OUT PACKAGE VERSION SDK_ROOT PACK_RECEIPT"

class Refusal < StandardError; end

def refuse(reason)
  raise Refusal, reason
end

def sha256(bytes)
  Digest::SHA256.hexdigest(bytes)
end

# Whether PATH is itself a regular file, not a link to one.
def regular_file?(path)
  File.lstat(path).file?
rescue SystemCallError
  false
end

# A value from the environment or a capture: unset and empty are unknown.
def known(value)
  value.nil? || value.empty? ? nil : value
end

def one_line(text)
  text.to_s.b.gsub(/\s*[\r\n]\s*/, " ").strip
end

# OUT must be absolute, name no parent directory, and lie outside the project PROJECT_ROOT, the directory this runs in
# unless retain-release names its SDK_ROOT. Its existing part is resolved, every link followed; its missing part would be
# made inside the deepest existing directory, so that directory and its ancestors are compared with the project by
# identity, not by spelling. This does not guard against another writer changing the path after the check.
def check_outside_project(out, project_root = Dir.pwd)
  refuse "#{out} is not an absolute path; the package is kept only at an absolute path outside the generated project" unless out.start_with?("/")
  parts = out.split("/").reject { |part| part.empty? || part == "." }
  refuse "#{out} names a parent directory (..); give the artifact directory without one" if parts.include?("..")
  root = File.realpath(project_root)
  directory = "/"
  parts.each do |part|
    candidate = File.join(directory, part)
    break unless File.exist?(candidate) || File.symlink?(candidate)

    directory = File.realpath(candidate)
  end
  loop do
    refuse "#{out} is inside the generated project #{root}; the package is kept only outside it" if File.identical?(directory, root)
    break if directory == "/"

    directory = File.dirname(directory)
  end
rescue SystemCallError => e
  refuse "#{out} cannot be resolved (#{e.class})"
end

def check_out(out)
  refuse "#{out} exists and is not an empty directory: the package is kept only where nothing else is" unless File.lstat(out).directory? && Dir.empty?(out)
rescue Errno::ENOENT
  nil
end

# The same-run public schema capture, checked against the .sha256 and .provenance.json capture-schema.sh wrote beside it.
def read_capture
  path = known(ENV["SWAGGER_CAPTURE"])
  refuse "SWAGGER_CAPTURE is not set: there is no same-run public schema capture to keep with the package" unless path
  path = File.expand_path(path)
  refuse "SWAGGER_CAPTURE #{path} is not a non-empty regular file" unless regular_file?(path) && File.size(path).positive?
  bytes = File.binread(path)
  digest = sha256(bytes)
  name = File.basename(path)

  sums_path = "#{path}.sha256"
  refuse "#{sums_path} is missing: the capture has no SHA-256 file" unless regular_file?(sums_path)
  sums = File.binread(sums_path)
  listed = sums.match(SHA256_LINE)
  refuse "#{sums_path} is not one line listing #{name} under a lowercase SHA-256" unless listed && listed[2] == name.b
  refuse "#{sums_path} lists SHA-256 #{listed[1]}, not the capture's #{digest}" unless listed[1] == digest

  record_path = "#{path}.provenance.json"
  refuse "#{record_path} is missing: the capture has no provenance" unless regular_file?(record_path)
  record_bytes = File.binread(record_path)
  begin
    record = JSON.parse(record_bytes.dup.force_encoding(Encoding::UTF_8))
  rescue JSON::ParserError, EncodingError
    refuse "#{record_path} is not JSON"
  end
  refuse "#{record_path} is not a JSON object" unless record.is_a?(Hash)
  unless record.keys.sort == CAPTURE_KEYS.sort
    refuse "#{record_path} has keys #{record.keys.sort.join(", ")}, not exactly #{CAPTURE_KEYS.sort.join(", ")}"
  end
  refuse "#{record_path} has format #{record["format"].inspect}, not #{CAPTURE_FORMAT}" unless record["format"] == CAPTURE_FORMAT
  refuse "#{record_path} has flavor #{record["flavor"].inspect}: only the public schema is kept with the package" unless record["flavor"] == "public"
  refuse "#{record_path} has source #{record["source"].inspect}, not #{CAPTURE_SOURCE}" unless record["source"] == CAPTURE_SOURCE
  refuse "#{record_path} records SHA-256 #{record["sha256"].inspect}, not the capture's #{digest}" unless record["sha256"] == digest
  unless record["bytes"].is_a?(Integer) && record["bytes"] == bytes.bytesize
    refuse "#{record_path} records #{record["bytes"].inspect} bytes, not the capture's #{bytes.bytesize}"
  end
  unless record["captured_at"].is_a?(String) && record["captured_at"].match?(UTC_SECOND)
    refuse "#{record_path} has captured_at #{record["captured_at"].inspect}, not a UTC time to the second"
  end
  %w[pipeline_id job_id commit_sha].each do |key|
    refuse "#{record_path} has #{key} #{record[key].inspect}, neither a string nor null" unless record[key].nil? || record[key].is_a?(String)
  end
  { name: name, bytes: bytes, sha256: digest, sums: sums, record: record, record_bytes: record_bytes }
end

def git_object(revision)
  output, errors, status = Open3.capture3("git", "rev-parse", "--verify", revision)
  refuse "#{Dir.pwd} is not a git checkout with a commit: git rev-parse #{revision} failed (#{one_line(errors)})" unless status.success?
  name = output.strip
  refuse "git rev-parse #{revision} printed #{name.inspect}, not an object name" unless name.match?(GIT_OBJECT)
  name
rescue Errno::ENOENT
  refuse "git is not installed: the source commit cannot be read"
end

# The commit and tree checked out here: the generator source the package was generated from.
def source_identity
  { "commit" => git_object("HEAD"), "tree" => git_object("HEAD^{tree}") }
end

# This build's CI identity, apart from the capture's.
def build_ci
  { "pipeline_id" => known(ENV["CI_PIPELINE_ID"]), "job_id" => known(ENV["CI_JOB_ID"]), "commit_sha" => known(ENV["CI_COMMIT_SHA"]) }
end

# How the capture, this build and the source join. Known values that differ refuse; where either side is unknown the
# join is unknown, never assumed. COMMIT_LABEL names where the build's commit came from.
def join_capture(record, source, ci, commit_label = "CI_COMMIT_SHA")
  captured_pipeline = known(record["pipeline_id"])
  captured_commit = known(record["commit_sha"])
  if captured_pipeline && ci["pipeline_id"] && captured_pipeline != ci["pipeline_id"]
    refuse "the schema was captured in pipeline #{captured_pipeline}, not this build's pipeline #{ci["pipeline_id"]}"
  end
  if captured_commit && captured_commit != source["commit"]
    refuse "the schema was captured at commit #{captured_commit}, not the source commit #{source["commit"]}"
  end
  if ci["commit_sha"] && ci["commit_sha"] != source["commit"]
    refuse "#{commit_label} #{ci["commit_sha"]} is not the source commit #{source["commit"]}"
  end
  { "pipeline" => captured_pipeline && ci["pipeline_id"] ? "same" : "unknown",
    "source_commit" => captured_commit && ci["commit_sha"] ? "same" : "unknown",
    "schema_sha256" => "same" }
end

# What check verifies, and retain verifies again: OUT is outside the project and can take the package, the capture is
# this run's public schema, and the capture, this build and the source join.
def preconditions(out)
  check_outside_project(out)
  check_out(out)
  capture = read_capture
  source = source_identity
  ci = build_ci
  [ capture, source, ci, join_capture(capture[:record], source, ci) ]
end

# The members of the ZIP archive DATA, name => content, read with zlib alone: the stored and deflated members of a
# single-disk archive, each checked against its CRC-32 and sizes. Anything else refuses the archive rather than guess.
def zip_members(data, label)
  refuse "#{label} is too short to be a ZIP archive" if data.bytesize < 22
  record = nil
  position = data.bytesize - 22
  floor = [ position - 0xFFFF, 0 ].max
  while position >= floor
    position = data.rindex(END_RECORD, position)
    break if position.nil? || position < floor
    if position + 22 + data.byteslice(position + 20, 2).unpack1("v") == data.bytesize
      record = position
      break
    end
    position -= 1
  end
  refuse "#{label} is not a ZIP archive: it has no end of central directory record" unless record

  _, disk, directory_disk, disk_entries, entries, directory_size, directory_offset = data.byteslice(record, 22).unpack("VvvvvVVv")
  if (record >= 20 && data.byteslice(record - 20, 4) == ZIP64_LOCATOR) || [ disk_entries, entries ].include?(0xFFFF) ||
     [ directory_size, directory_offset ].include?(0xFFFFFFFF)
    refuse "#{label} is a ZIP64 archive, which is not read"
  end
  refuse "#{label} spans more than one disk" unless disk.zero? && directory_disk.zero? && disk_entries == entries
  refuse "#{label}'s central directory is not where its end record places it" unless directory_offset + directory_size == record

  members = {}
  position = directory_offset
  entries.times do
    header = data.byteslice(position, 46)
    refuse "#{label}'s central directory is cut short" unless header && header.bytesize == 46 && header.start_with?(CENTRAL_HEADER)
    fields = header.unpack("VvvvvvvVVVvvvvvVV")
    flags, method, crc, compressed_size, size, name_length, extra_length, comment_length, start_disk, offset = fields.values_at(3, 4, 7, 8, 9, 10, 11, 12, 13, 16)
    name = data.byteslice(position + 46, name_length).force_encoding(Encoding::UTF_8)
    position += 46 + name_length + extra_length + comment_length
    refuse "#{label}'s central directory runs past its end record" if position > record
    refuse "#{label} has a member name that is not UTF-8" unless name.valid_encoding?
    refuse "#{label} holds #{name} more than once" if members.key?(name)
    refuse "#{label}'s #{name} is a ZIP64 member, which is not read" if [ compressed_size, size, offset ].include?(0xFFFFFFFF) || start_disk == 0xFFFF
    refuse "#{label}'s #{name} is on another disk" unless start_disk.zero?
    refuse "#{label}'s #{name} is encrypted" if flags.anybits?(ENCRYPTED)
    refuse "#{label}'s #{name} uses compression method #{method}; only stored (0) and deflated (8) are read" unless [ 0, 8 ].include?(method)
    members[name] = member_content(data, label, name, method, crc, compressed_size, size, offset, directory_offset)
  end
  refuse "#{label}'s central directory does not end where its end record begins" unless position == record
  members
end

def member_content(data, label, name, method, crc, compressed_size, size, offset, limit)
  header = data.byteslice(offset, 30)
  refuse "#{label}'s #{name} has no local header where its central directory entry points" unless header && header.bytesize == 30 && header.start_with?(LOCAL_HEADER)
  _, _, flags, local_method, _, _, local_crc, local_compressed_size, local_size, name_length, extra_length = header.unpack("VvvvvvVVVvv")
  agrees = data.byteslice(offset + 30, name_length) == name.b && local_method == method && !flags.anybits?(ENCRYPTED) &&
           (flags.anybits?(DATA_DESCRIPTOR) || [ local_crc, local_compressed_size, local_size ] == [ crc, compressed_size, size ])
  refuse "#{label}'s #{name} has a local header that disagrees with its central directory entry" unless agrees
  start = offset + 30 + name_length + extra_length
  refuse "#{label}'s #{name} runs into the central directory" if start + compressed_size > limit
  compressed = data.byteslice(start, compressed_size)
  content = method.zero? ? compressed : inflate(compressed, label, name)
  refuse "#{label}'s #{name} is #{content.bytesize} bytes, not the #{size} its central directory records" unless content.bytesize == size
  refuse "#{label}'s #{name} fails its CRC-32 check" unless Zlib.crc32(content) == crc
  content
end

def inflate(compressed, label, name)
  inflater = Zlib::Inflate.new(-Zlib::MAX_WBITS)
  content = inflater.inflate(compressed)
  unless inflater.finished? && inflater.total_in == compressed.bytesize
    refuse "#{label}'s #{name} is not one whole deflate stream of its recorded compressed size"
  end
  content
rescue Zlib::Error => e
  refuse "#{label}'s #{name} does not inflate (#{e.message})"
ensure
  inflater&.close
end

# The recipe's commands in the order build.sh ran them; every one must have exited 0.
def recipe_steps(path)
  refuse "STEPS #{path} is not a regular file" unless regular_file?(path)
  text = File.binread(path).force_encoding(Encoding::UTF_8)
  refuse "STEPS #{path} is not UTF-8 text" unless text.valid_encoding?
  lines = text.split("\n", -1)
  lines.pop if lines.last == ""
  refuse "STEPS #{path} records no step" if lines.empty?
  lines.each_with_index.map do |line, index|
    step = line.match(/\A([^\t\r]+)\t(-?\d+)\z/)
    refuse "STEPS line #{index + 1} is not NAME<TAB>EXIT: #{line.inspect}" unless step
    refuse "STEPS line #{index + 1}: #{step[1]} exited #{step[2]}, so the recipe did not succeed" unless step[2] == "0"
    { "name" => step[1], "exit" => 0 }
  end
end

def collect_files(directory, files)
  Dir.children(directory).each do |child|
    path = File.join(directory, child)
    stat = File.lstat(path)
    if stat.directory?
      collect_files(path, files) unless %w[bin obj].include?(child)
    elsif stat.file?
      files << path
    end
  end
end

# The "SHA256  PATH" lines of every generated input that was packed, read now, sorted by path: those of the directory
# this runs in, or of ROOT when given, by paths relative to it.
def generated_input_manifest(root = nil)
  return Dir.chdir(root) { generated_input_manifest } if root

  refuse "#{File.expand_path(PROJECT)} is not a directory: there is no generated source to account for" unless File.directory?(PROJECT) && !File.symlink?(PROJECT)
  files = []
  collect_files(PROJECT, files)
  refuse "#{File.expand_path(PROJECT)} holds no file" if files.empty?
  files.concat(TOP_INPUTS.select { |path| regular_file?(path) })
  files.sort.map do |path|
    refuse "the generated input #{path.inspect} has a line break in its name" if path.b.match?(/[\r\n]/)
    "#{Digest::SHA256.file(path).hexdigest}  #{path}\n"
  end
end

# The strings in VALUE, keys included, that name an absolute path. A Base64 digest may hold a slash and is not a path.
def absolute_paths(value, key = nil)
  case value
  when Hash then value.flat_map { |k, v| absolute_paths(k) + absolute_paths(v, k) }
  when Array then value.flat_map { |item| absolute_paths(item, key) }
  when String then key != "sha512_base64" && value.match?(ABSOLUTE_PATH) ? [ value ] : []
  else []
  end
end

def check(out)
  capture, = preconditions(out)
  puts "package-provenance: #{out} can take the package; the public schema capture #{capture[:sha256]} joins this checkout and build"
end

# PACKAGE, the archive dotnet pack wrote for VERSION, checked and read: its bytes, its lib/ members' SHA-256s and its
# target frameworks.
def read_package(package, version)
  refuse "VERSION #{version.inspect} is not a package version" unless version.match?(PACKAGE_VERSION)
  package_name = "FilesCom.#{version}.nupkg"
  refuse "PACKAGE #{package} is not named #{package_name}" unless File.basename(package) == package_name
  refuse "PACKAGE #{package} is not a regular file" unless regular_file?(package)
  archive = File.binread(package)
  members = zip_members(archive, package_name)
  nuspec = members["FilesCom.nuspec"]
  refuse "#{package_name} has no FilesCom.nuspec at its root" unless nuspec
  nuspec = nuspec.dup.force_encoding(Encoding::UTF_8).scrub
  ids = nuspec.scan(%r{<id>\s*([^<]*?)\s*</id>}).flatten
  versions = nuspec.scan(%r{<version>\s*([^<]*?)\s*</version>}).flatten
  refuse "#{package_name}'s FilesCom.nuspec has id #{ids.join(", ").inspect}, not FilesCom" unless ids == [ "FilesCom" ]
  refuse "#{package_name}'s FilesCom.nuspec has version #{versions.join(", ")}, not #{version}" unless versions == [ version ]
  lib_members = members.keys.select { |name| name.start_with?("lib/") && !name.end_with?("/") }.sort.to_h { |name| [ name, sha256(members[name]) ] }
  frameworks = lib_members.keys.map { |name| name.split("/") }.select { |parts| parts.size > 2 }.map { |parts| parts[1] }.uniq.sort
  refuse "#{package_name} has no lib/FRAMEWORK/ member" if frameworks.empty?
  { name: package_name, version: version, archive: archive, lib_members: lib_members, frameworks: frameworks }
end

# The files kept in OUT beside the provenance, path => bytes.
def copies_for(package, capture, manifest)
  name = capture[:name]
  {
    package[:name] => package[:archive],
    "schema/#{name}" => capture[:bytes],
    "schema/#{name}.sha256" => capture[:sums],
    "schema/#{name}.provenance.json" => capture[:record_bytes],
    "generated-input.sha256" => manifest
  }
end

# The provenance of PACKAGE: its producer and what it does not claim, then the same record for either mode. EXTRA is
# added after build.
def provenance_record(producer, not_claimed, package, source, capture, manifest_lines, steps, ci, joined, extra = {})
  archive = package[:archive]
  manifest = manifest_lines.join
  name = capture[:name]
  {
    "format" => FORMAT,
    "at" => Time.now.utc.strftime("%Y-%m-%dT%H:%M:%SZ"),
    "producer" => producer,
    "paths" => "relative to the directory holding this file",
    "package" => {
      "id" => "FilesCom",
      "version" => package[:version],
      "target_frameworks" => package[:frameworks],
      "archive" => {
        "path" => package[:name],
        "sha256" => sha256(archive),
        "bytes" => archive.bytesize,
        "sha512_hex" => Digest::SHA512.hexdigest(archive),
        "sha512_base64" => [ Digest::SHA512.digest(archive) ].pack("m0")
      },
      "lib_members" => package[:lib_members]
    },
    "source" => source,
    "schema" => {
      "path" => "schema/#{name}",
      "sha256" => capture[:sha256],
      "bytes" => capture[:bytes].bytesize,
      "capture_provenance" => capture[:record],
      "capture_provenance_sha256" => sha256(capture[:record_bytes]),
      "capture_sha256_file" => { "path" => "schema/#{name}.sha256", "sha256" => sha256(capture[:sums]) }
    },
    "generated_input" => {
      "manifest" => { "path" => "generated-input.sha256", "sha256" => sha256(manifest), "bytes" => manifest.bytesize, "files" => manifest_lines.size }
    },
    "build" => { "commands" => steps, "ci" => ci }
  }.merge(extra).merge("join" => joined, "not_claimed" => not_claimed)
end

# Writes COPIES and then the provenance to OUT, after its last check: no absolute path in it.
def keep(out, copies, provenance, package)
  absolute = absolute_paths(provenance)
  refuse "the provenance would hold the absolute path #{absolute.first.inspect}; it names files only relative to itself" if absolute.any?
  text = "#{JSON.pretty_generate(provenance)}\n"

  # Nothing is written before this point.
  FileUtils.mkdir_p(File.join(out, "schema"))
  copies.each { |path, bytes| File.binwrite(File.join(out, path), bytes) }
  copies.each do |path, bytes|
    next if Digest::SHA256.file(File.join(out, path)).hexdigest == sha256(bytes)

    refuse "#{File.join(out, path)} does not hash as the bytes that were checked; the copies stay, and no provenance is written"
  end
  temporary = File.join(out, ".package-provenance.json.#{Process.pid}.tmp")
  begin
    File.binwrite(temporary, text)
    File.rename(temporary, File.join(out, "package-provenance.json"))
  rescue StandardError
    FileUtils.rm_f(temporary)
    raise
  end
  puts "package-provenance: kept #{package[:name]} (SHA-256 #{sha256(package[:archive])}) with its schema capture and package-provenance.json in #{out}"
end

def retain(out, package_path, version, steps_path)
  capture, source, ci, joined = preconditions(out)
  package = read_package(package_path, version)
  steps = recipe_steps(steps_path)
  manifest_lines = generated_input_manifest
  copies = copies_for(package, capture, manifest_lines.join)
  keep(out, copies, provenance_record(PRODUCER, NOT_CLAIMED, package, source, capture, manifest_lines, steps, ci, joined), package)
end

# HASH, checked to be a JSON object with exactly the keys KEYS, and any of OPTIONAL.
def exact_keys(hash, label, keys, optional = [])
  refuse "#{label} is not a JSON object" unless hash.is_a?(Hash)
  unless (keys - hash.keys).empty? && (hash.keys - keys - optional).empty?
    expected = keys + optional.map { |key| "#{key} (optional)" }
    refuse "#{label} has keys #{hash.keys.sort.join(", ")}, not exactly #{expected.join(", ")}"
  end
  hash
end

def object_name(value, label)
  refuse "#{label} #{value.inspect} is not a git object name" unless value.is_a?(String) && value.match?(GIT_OBJECT)
  value
end

# The release job's receipt of its pack, checked for its shape: its bytes, source, commands, CI identity, package and
# manifest records, and mirror (or nil).
def read_receipt(path)
  refuse "PACK_RECEIPT #{path} is not a regular file" unless regular_file?(path)
  bytes = File.binread(path)
  text = bytes.dup.force_encoding(Encoding::UTF_8)
  refuse "PACK_RECEIPT #{path} is not UTF-8 text" unless text.valid_encoding?
  begin
    receipt = JSON.parse(text)
  rescue JSON::ParserError
    refuse "PACK_RECEIPT #{path} is not JSON"
  end
  exact_keys(receipt, "PACK_RECEIPT", %w[format source build package generated_input], %w[mirror])
  refuse "PACK_RECEIPT has format #{receipt["format"].inspect}, not #{RECEIPT_FORMAT}" unless receipt["format"] == RECEIPT_FORMAT

  source = exact_keys(receipt["source"], "PACK_RECEIPT source", %w[commit tree])
  source = { "commit" => object_name(source["commit"], "PACK_RECEIPT source.commit"), "tree" => object_name(source["tree"], "PACK_RECEIPT source.tree") }

  build = exact_keys(receipt["build"], "PACK_RECEIPT build", %w[commands ci])
  commands = build["commands"]
  refuse "PACK_RECEIPT build.commands is not a non-empty array" unless commands.is_a?(Array) && !commands.empty?
  steps = commands.each_with_index.map do |command, index|
    label = "PACK_RECEIPT build.commands[#{index}]"
    exact_keys(command, label, %w[name exit])
    name = command["name"]
    refuse "#{label}.name #{name.inspect} is not non-empty text on one line" unless name.is_a?(String) && !name.strip.empty? && !name.match?(/[[:cntrl:]]/)
    refuse "#{label}.exit #{command["exit"].inspect} is not an integer" unless command["exit"].is_a?(Integer)
    refuse "#{label}: #{name} exited #{command["exit"]}, so the pack did not succeed" unless command["exit"].zero?
    { "name" => name, "exit" => 0 }
  end
  recorded_ci = exact_keys(build["ci"], "PACK_RECEIPT build.ci", %w[pipeline_id job_id commit_sha])
  ci = %w[pipeline_id job_id commit_sha].to_h do |key|
    value = recorded_ci[key]
    refuse "PACK_RECEIPT build.ci.#{key} #{value.inspect} is neither a string nor null" unless value.nil? || value.is_a?(String)
    [ key, known(value) ]
  end

  package = exact_keys(receipt["package"], "PACK_RECEIPT package", %w[id version sha256 bytes])
  manifest = exact_keys(exact_keys(receipt["generated_input"], "PACK_RECEIPT generated_input", %w[manifest])["manifest"],
                        "PACK_RECEIPT generated_input.manifest", %w[sha256 bytes files])

  mirror = nil
  if receipt.key?("mirror")
    recorded = exact_keys(receipt["mirror"], "PACK_RECEIPT mirror", %w[commit tag])
    tag = recorded["tag"]
    refuse "PACK_RECEIPT mirror.tag #{tag.inspect} is neither non-empty text nor null" unless tag.nil? || (tag.is_a?(String) && !tag.strip.empty?)
    mirror = { "commit" => object_name(recorded["commit"], "PACK_RECEIPT mirror.commit"), "tag" => tag }
  end
  { bytes: bytes, source: source, steps: steps, ci: ci, package: package, manifest: manifest, mirror: mirror }
end

# The release-local pack of retain-release: PACKAGE as PACK_RECEIPT records it, from SDK_ROOT.
def retain_release(out, package_path, version, sdk_root, receipt_path)
  package_path = File.expand_path(package_path)
  receipt_path = File.expand_path(receipt_path)
  sdk_root = File.expand_path(sdk_root)
  refuse "SDK_ROOT #{sdk_root} is not a directory" unless File.directory?(sdk_root)
  check_outside_project(out, sdk_root)
  check_out(out)
  receipt = read_receipt(receipt_path)
  capture = read_capture
  joined = join_capture(capture[:record], receipt[:source], receipt[:ci], "the receipt's build.ci.commit_sha")

  package = read_package(package_path, version)
  recorded = receipt[:package]
  refuse "PACK_RECEIPT package.id #{recorded["id"].inspect} is not FilesCom" unless recorded["id"] == "FilesCom"
  refuse "PACK_RECEIPT package.version #{recorded["version"].inspect} is not VERSION #{version}" unless recorded["version"] == version
  unless recorded["sha256"] == sha256(package[:archive]) && recorded["bytes"] == package[:archive].bytesize
    refuse "PACK_RECEIPT records package SHA-256 #{recorded["sha256"].inspect} and #{recorded["bytes"].inspect} bytes, not PACKAGE's #{sha256(package[:archive])} and #{package[:archive].bytesize}"
  end

  manifest_lines = generated_input_manifest(sdk_root)
  manifest = manifest_lines.join
  recorded = receipt[:manifest]
  unless recorded["sha256"] == sha256(manifest) && recorded["bytes"] == manifest.bytesize && recorded["files"] == manifest_lines.size
    refuse "PACK_RECEIPT records a generated-input manifest of SHA-256 #{recorded["sha256"].inspect}, #{recorded["bytes"].inspect} bytes and " \
           "#{recorded["files"].inspect} files, not SDK_ROOT's #{sha256(manifest)}, #{manifest.bytesize} and #{manifest_lines.size}"
  end

  release_pack = {
    "release_pack" => {
      "receipt" => { "format" => RECEIPT_FORMAT, "sha256" => sha256(receipt[:bytes]), "bytes" => receipt[:bytes].bytesize },
      "mirror" => receipt[:mirror]
    }
  }
  provenance = provenance_record(RELEASE_PRODUCER, RELEASE_NOT_CLAIMED, package, receipt[:source], capture, manifest_lines,
                                 receipt[:steps], receipt[:ci], joined, release_pack)
  keep(out, copies_for(package, capture, manifest), provenance, package)
end

command, *arguments = ARGV
arity = { "check" => 1, "retain" => 4, "retain-release" => 5 }[command]
unless arity && arguments.size == arity && arguments.none?(&:empty?)
  warn "package-provenance: #{USAGE}"
  exit 2
end

begin
  case command
  when "check" then check(*arguments)
  when "retain" then retain(*arguments)
  else retain_release(*arguments)
  end
rescue Refusal => e
  warn "package-provenance: #{one_line(e.message)}"
  exit 1
rescue StandardError => e
  warn "package-provenance: #{e.class}: #{one_line(e.message)}"
  exit 1
end
