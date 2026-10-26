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
    --logger "trx;LogFilePrefix=FilesTests" --results-directory "${results}" || exit 1
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
    dotnet test FilesTests/FilesTests.csproj --framework "${framework}" \
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
