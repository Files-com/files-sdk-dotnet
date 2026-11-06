#!/usr/bin/env bash
set -euo pipefail
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
mock="${1:?generated mock server required}"
out="${2:?output directory required}"
frameworks="${3:?target frameworks required}"
ran=0
for framework in ${frameworks//;/ }; do
  run="${out}/${framework}"
  [[ ! -e "${run}" ]] || { echo "${run} already exists" >&2; exit 1; }
  mkdir -p "${run}"
  ruby "${mock}/with-scenario.rb" --scenario api-scenarios-canonical --out "${run}/launcher" \
    --startup-timeout 30 --consumer-timeout 90 --settle-timeout 5 -- \
    dotnet test "${DIR}/sdk/FilesTests/FilesTests.csproj" --no-build --framework "${framework}" \
    --filter "TestCategory=SharedSimulatorList" --logger "trx;LogFileName=lists.trx" --results-directory "${run}"
  ruby -rrexml/document -e '
    path = ARGV.fetch(0)
    document = REXML::Document.new(File.read(path))
    expected = %w[TestSimpleList TestNotFoundList TestEmptyList].map { |name| "FilesTests.FilesApiListTest.#{name}" }.sort
    tests = REXML::XPath.match(document, "//UnitTest").to_h do |test|
      method = test.elements["TestMethod"]
      [test.attributes["id"], method && "#{method.attributes["className"]}.#{method.attributes["name"]}"]
    end
    results = REXML::XPath.match(document, "//UnitTestResult")
    counters = REXML::XPath.first(document, "//ResultSummary/Counters")
    abort("#{path}: did not discover precisely the three legacy list tests") unless tests.values.sort == expected
    abort("#{path}: did not execute and pass every list test exactly once") unless results.size == 3 &&
      results.map { |result| result.attributes["testId"] }.sort == tests.keys.sort &&
      results.all? { |result| result.attributes["outcome"] == "Passed" } &&
      %w[total executed passed].all? { |key| counters && counters.attributes[key] == "3" }
    puts "#{path}: all three legacy folder-list tests passed"
  ' "${run}/lists.trx"
  ran=$((ran + 1))
done
[[ "${ran}" -gt 0 ]] || { echo "No target framework was tested" >&2; exit 1; }
