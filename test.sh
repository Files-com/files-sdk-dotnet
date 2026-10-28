#!/usr/bin/env bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
# Each target framework runs on its own runtime, which must be installed.
unset DOTNET_ROLL_FORWARD

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd "${DIR}/sdk" || exit 1

# The generated contract cases run against the API simulator generated from the same schema, through a package of
# this library packed here, as applications install it. FILES_CONTRACT_MOCK_SERVER names the simulator's directory:
# the generated mock-server target (simulation contract 3) with its bundle installed. Without it the run fails, before
# anything else runs.
if [[ -z "${FILES_CONTRACT_MOCK_SERVER:-}" ]]; then
  echo "FILES_CONTRACT_MOCK_SERVER must name the generated simulator the contract cases run against" >&2
  exit 1
fi

# Results stay outside the SDK tree, which is published as is. A run that passes removes them; one that fails keeps them
# and names them, since the contract stage's simulator.log and contract-results.json explain its failure.
results="$(mktemp -d)" || exit 1
trap 'status=$?; if [[ ${status} -eq 0 ]]; then rm -rf "${results}"; else echo "test.sh failed (exit ${status}); its results are kept in ${results}" >&2; fi' EXIT

frameworks="$(dotnet msbuild FilesTests/FilesTests.csproj -getProperty:TargetFrameworks)" || exit 1
# Every test but the one that runs against a simulator of its own (TestCategory SharedSimulatorFilesystem), which runs
# last, under the simulator's launcher.
ordinary_tests="TestCategory!=SharedSimulatorFilesystem"

# The frameworks are tested one after another. A CI job (CI=true) can opt in (FILES_DOTNET_TEST_FRAMEWORKS_IN_PARALLEL=1)
# to testing them at the same time when it has the CPUs and memory for it, which parallel-test-resources.sh decides
# before any test runs; otherwise, and outside CI, they run one after another as without the opt-in. Either way a
# failure fails the run: nothing is retried the other way.
parallel=""
case "${FILES_DOTNET_TEST_FRAMEWORKS_IN_PARALLEL:-}" in
  "") ;;
  1)
    if [[ "${CI:-}" == true ]]; then
      "${DIR}/parallel-test-resources.sh" && parallel=1
    else
      echo "sequential: FILES_DOTNET_TEST_FRAMEWORKS_IN_PARALLEL applies in CI (CI=true) only"
    fi
    ;;
  *)
    echo "FILES_DOTNET_TEST_FRAMEWORKS_IN_PARALLEL must be 1 or unset, not ${FILES_DOTNET_TEST_FRAMEWORKS_IN_PARALLEL}" >&2
    exit 1
    ;;
esac
if [[ -n "${parallel}" ]]; then
  # One run tests every framework, each in its own test host on its own runtime, at the same time (the SDK's
  # TestTfmsInParallel). LogFilePrefix gives each framework its own TRX file, FilesTests_<framework>_<timestamp>.trx.
  # Each host also gets a temporary directory of its own, results/tmp/<framework>, made like the system's (mode 1777),
  # as TMPDIR (FilesTests.csproj, FilesTestsFrameworkTemp): the hosts would otherwise share one, and a test that looks
  # at what is in it would also see the other host's files. Like the rest of the results, they are removed when the run
  # passes and kept when it fails.
  mkdir -m 1777 "${results}/tmp" || exit 1
  for framework in ${frameworks//;/ }; do
    mkdir -m 1777 "${results}/tmp/${framework}" || exit 1
  done
  dotnet test FilesTests/FilesTests.csproj -p:TestTfmsInParallel=true -p:FilesTestsFrameworkTemp="${results}/tmp" \
    --filter "${ordinary_tests}" --logger "trx;LogFilePrefix=FilesTests" --results-directory "${results}" || exit 1
fi
for framework in ${frameworks//;/ }; do
  if [[ -n "${parallel}" ]]; then
    reports=("${results}"/FilesTests_"${framework}"_*.trx)
    if [[ "${#reports[@]}" -ne 1 || ! -f "${reports[0]}" ]]; then
      echo "Expected one TRX file for ${framework} in ${results}" >&2
      exit 1
    fi
    report="${reports[0]}"
  else
    dotnet test FilesTests/FilesTests.csproj --framework "${framework}" --filter "${ordinary_tests}" \
      --logger "trx;LogFileName=${framework}.trx" --results-directory "${results}" || exit 1
    report="${results}/${framework}.trx"
  fi

  # A run that finds no tests passes; fail it here instead.
  counters="$(grep -o '<Counters [^>]*>' "${report}")"
  total="$(sed -E 's/.* total="([0-9]+)".*/\1/' <<< "${counters}")"
  executed="$(sed -E 's/.* executed="([0-9]+)".*/\1/' <<< "${counters}")"
  echo "${framework}: ${total} tests found, ${executed} executed, $((total - executed)) skipped"
  if [[ -z "${executed}" || "${executed}" -eq 0 ]]; then
    echo "No tests executed on ${framework}" >&2
    exit 1
  fi
done

dotnet pack FilesCom/FilesCom.csproj --configuration Production --output "${results}/package" || exit 1
packages=("${results}"/package/FilesCom.*.nupkg)
if [[ "${#packages[@]}" -ne 1 || ! -f "${packages[0]}" ]]; then
  echo "Expected one FilesCom package in ${results}/package" >&2
  exit 1
fi
package_sha256="$(ruby -rdigest -e 'puts Digest::SHA256.file(ARGV[0]).hexdigest' "${packages[0]}")" || exit 1
ruby "${DIR}/contract-test.rb" "${FILES_CONTRACT_MOCK_SERVER}" "${results}/contract" "${packages[0]}" "${package_sha256}" || exit 1

# The shared API scenarios (verification/scenarios/api.json), generated as sdk/FilesApiScenarioTests, run against the same
# archive, one case at a time on each framework, each against a simulator of its own that the mock server's
# with-scenario.rb starts (sdk/FilesApiScenarioTests/README.md). Their results go to FILES_API_SCENARIOS_OUT when it is
# set, an absent or empty directory outside this SDK, as CI sets it to keep them; otherwise they stay with these results.
ruby "${DIR}/api-scenarios.rb" "${FILES_CONTRACT_MOCK_SERVER}" "${FILES_API_SCENARIOS_OUT:-${results}/api-scenarios}" "${packages[0]}" "${package_sha256}" || exit 1

# client.FileSystem runs against a real simulator rather than the loopback fakes of the runs above, which leave out the
# two tests of TestCategory SharedSimulatorFilesystem: FilesTests.RemoteFileSystemTest.WholeFilesAreWrittenAndRead,
# which writes a whole file and reads it back, then ExistenceAndFolderCreationFollowWhatIsThere, which asks what is
# at a path and creates folders. Once the shared API scenarios have returned, on each framework, the two run one after
# the other, each alone: the mock server's with-scenario.rb starts a fresh simulator under the same
# api-scenarios-canonical preset, with the API scenarios' bounds (30 seconds to start, 210 for the test, 5 to stop each
# process group), and runs that one test as the runs above built it, from this SDK's source, not the package. The test
# checks the simulator the launcher names and resets it itself, empty or to the folder and file it starts from. It
# passes only when the launcher exits 0 (the test passed, the journal is complete and every process group it started is
# gone) and its TRX shows that one test, discovered once, with one passed result: total, executed and passed 1. Both
# tests must pass on every framework, in that order. Their results go to filesystem-helpers/<framework>/<test> beside
# the API scenarios' own, made only now that those have returned, and are no part of them.
filesystem_tests=(WholeFilesAreWrittenAndRead ExistenceAndFolderCreationFollowWhatIsThere)
filesystem_out="${FILES_API_SCENARIOS_OUT:-${results}/api-scenarios}/filesystem-helpers"
filesystem_expected=()
for framework in ${frameworks//;/ }; do
  for name in "${filesystem_tests[@]}"; do
    filesystem_expected+=("${framework}/${name}")
  done
done
filesystem_passed=()
for framework in ${frameworks//;/ }; do
  for name in "${filesystem_tests[@]}"; do
    filesystem_test="FilesTests.RemoteFileSystemTest.${name}"
    filesystem_run="${filesystem_out}/${framework}/${name}"
    mkdir -p "${filesystem_run}" || exit 1
    ruby "${FILES_CONTRACT_MOCK_SERVER}/with-scenario.rb" --scenario api-scenarios-canonical --out "${filesystem_run}/launcher" \
      --startup-timeout 30 --consumer-timeout 210 --settle-timeout 5 -- \
      dotnet test "${DIR}/sdk/FilesTests/FilesTests.csproj" --no-build --framework "${framework}" \
      --filter "FullyQualifiedName=${filesystem_test}" --logger "trx;LogFileName=filesystem.trx" --results-directory "${filesystem_run}"
    launcher_status=$?
    if [[ "${launcher_status}" -ne 0 ]]; then
      echo "${filesystem_test} on ${framework}: the launcher exited ${launcher_status}; see ${filesystem_run}" >&2
      exit 1
    fi
    ruby -rrexml/document -e '
      trx, expected = ARGV
      abort("#{trx}: no TRX was written") unless File.file?(trx)
      document = REXML::Document.new(File.read(trx))
      tests = REXML::XPath.match(document, "//UnitTest").map do |test|
        method = test.elements["TestMethod"]
        [ test.attributes["id"], method && "#{method.attributes["className"]}.#{method.attributes["name"]}" ]
      end
      results = REXML::XPath.match(document, "//UnitTestResult").map { |result| [ result.attributes["testId"], result.attributes["outcome"] ] }
      counters = REXML::XPath.first(document, "//ResultSummary/Counters")
      counts = %w[total executed passed].to_h { |name| [ name, counters && counters.attributes[name] ] }
      problems = []
      problems << "it discovered #{tests.map(&:last).inspect}, not #{expected} once" unless tests.size == 1 && tests[0][1] == expected && !tests[0][0].to_s.empty?
      problems << "its results are #{results.inspect}, not one passed result of that test" unless results.size == 1 && results[0] == [ tests.dig(0, 0), "Passed" ]
      problems << "its counters are #{counts}, not total, executed and passed 1" unless counts.values.all?("1")
      abort("#{trx}: #{problems.join("; ")}") unless problems.empty?
    ' "${filesystem_run}/filesystem.trx" "${filesystem_test}" || exit 1
    filesystem_passed+=("${framework}/${name}")
  done
done
if [[ "${#filesystem_passed[@]}" -eq 0 || "${filesystem_passed[*]}" != "${filesystem_expected[*]}" ]]; then
  echo "FilesTests.RemoteFileSystemTest passed ${filesystem_passed[*]:-nothing}, not each of ${filesystem_expected[*]:-its simulator tests on ${frameworks}} in turn" >&2
  exit 1
fi
echo "FilesTests.RemoteFileSystemTest: passed against the simulator as ${filesystem_passed[*]}"
