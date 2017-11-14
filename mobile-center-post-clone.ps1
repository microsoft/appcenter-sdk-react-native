#!/usr/bin/env bash
echo "Executing post clone script in $pwd"
(Get-Content NuGet.config).replace('NUGET_PASSWORD', $env:NUGET_PASSWORD) | Set-Content NuGet.config
[System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($env:GOOGLE_SERVICES_JSON)) | Set-Content Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/google-services.json
