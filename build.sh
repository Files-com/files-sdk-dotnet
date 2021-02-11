#!/usr/bin/env bash

cd sdk/Files.com || exit
dotnet build --configuration Release
cd ../../
