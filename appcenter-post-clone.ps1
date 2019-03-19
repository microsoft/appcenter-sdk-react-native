#!/usr/bin/env bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

echo "Executing post clone script in $pwd"
(Get-Content NuGet.config).replace('NUGET_PASSWORD', $env:NUGET_PASSWORD) | Set-Content NuGet.config
