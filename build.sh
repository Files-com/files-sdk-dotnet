#!/usr/bin/env bash

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

cd sdk/FilesCom || exit 1
dotnet format || exit 1
dotnet format --verify-no-changes . || exit 1
dotnet build --configuration Production || exit 1
cd ../../ || exit 1
