#!/usr/bin/env bash

cd sdk/FilesCom || exit 1
dotnet build --configuration Production || exit 1
cd ../../ || exit 1
