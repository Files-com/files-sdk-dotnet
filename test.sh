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
for framework in ${frameworks//;/ }; do
  dotnet test FilesTests/FilesTests.csproj --framework "${framework}" \
    --logger "trx;LogFileName=${framework}.trx" --results-directory "${results}" || exit 1

  # A run that finds no tests passes; fail it here instead.
  counters="$(grep -o '<Counters [^>]*>' "${results}/${framework}.trx")"
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
