#!/usr/bin/env bash

# Skip sending telemetry and first time experience
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd "${DIR}" || exit 1

# With FILES_DOTNET_CI_ARTIFACT_DIR set, the package this run builds is kept there, with package-provenance.json and the
# same-run public schema capture SWAGGER_CAPTURE names, once every step below has succeeded (package-provenance.rb).
# Before anything is built, package-provenance.rb checks that the directory is an absolute path that, links followed,
# lies outside this generated project (the SDK CI keeps and publishes), that it is absent or empty, and the capture.
# Without it, nothing is kept: the package lives only in this run's work directory.
artifact="${FILES_DOTNET_CI_ARTIFACT_DIR:-}"
if [[ -n "${artifact}" ]]; then
  ruby "${DIR}/package-provenance.rb" check "${artifact}" || exit 1
fi

work="$(mktemp -d)" || exit 1
trap 'rm -rf "${work}"' EXIT
steps="${work}/steps.tsv"

# Runs one command of the recipe; a failure ends the build. When the package is to be kept, the command's exit is also
# recorded for its provenance, and a record that cannot be written ends the build too, so the provenance never lists
# fewer commands than ran. A command's own failure stays the first one reported.
step() {
  local status=0 name="$*" shown='$work'
  "$@" || status=$?
  if [[ -n "${artifact}" ]]; then
    name="${name//"${work}"/${shown}}"
    if ! printf '%s\t%s\n' "${name}" "${status}" >> "${steps}"; then
      [[ ${status} -ne 0 ]] || echo "The exit of \"${name}\" could not be recorded in ${steps}; the build stops." >&2
      exit 1
    fi
  fi
  [[ ${status} -eq 0 ]] || exit 1
}

format() {
  step dotnet format "$1"
  step dotnet format --verify-no-changes "$1"
}

format sdk/FilesCom/FilesCom.csproj
format sdk/FilesTests/FilesTests.csproj
format sdk/FilesContractTests/FilesContractTests.csproj
step dotnet build sdk/FilesCom/FilesCom.csproj --configuration Production

# The examples build against a package of this library, as applications install it. Restores take FilesCom only
# from that package and use their own package folder, so a published FilesCom cannot stand in for it.
step dotnet pack sdk/FilesCom/FilesCom.csproj --configuration Production --no-build --output "${work}/packages"
cat > "${work}/NuGet.Config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="${work}/packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="FilesCom" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
  <auditSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </auditSources>
</configuration>
EOF
FilesComVersion="$(dotnet msbuild sdk/FilesCom/FilesCom.csproj -getProperty:Version)" || exit 1
export FilesComVersion
export RestoreConfigFile="${work}/NuGet.Config"
export NUGET_PACKAGES="${work}/restored"
for example in examples/*/*.csproj; do
  format "${example}"
  step dotnet build "${example}" --configuration Release
done

if [[ -n "${artifact}" ]]; then
  ruby "${DIR}/package-provenance.rb" retain "${artifact}" "${work}/packages/FilesCom.${FilesComVersion}.nupkg" "${FilesComVersion}" "${steps}" || exit 1
fi
