#!/usr/bin/env bash
echo "Executing post clone script in $pwd"
(Get-Content NuGet.config).replace('NUGET_PASSWORD', $env:NUGET_PASSWORD) | Set-Content NuGet.config
