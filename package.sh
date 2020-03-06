#!/usr/bin/env bash

rm -rf ./package
dotnet pack ./src/GraphiQl/GraphiQl.csproj  -o ./package/ -c release