#!/usr/bin/env bash

cd sdk/FilesCom || exit
dotnet build --configuration Release
cd ../../
