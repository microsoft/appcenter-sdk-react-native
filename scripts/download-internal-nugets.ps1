# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

[CmdletBinding()]
Param(
    [string]$NugetPassword
)

$basePackage="Microsoft.AppCenter"
$version=(Select-String -Path windows/nuspecs/nuget/AppCenter.nuspec -Pattern "version>(.*)<").Matches.Groups[1].Value
foreach ($packageSuffix in @("", ".Analytics", ".Crashes", ".Distribute", ".Push")) {
  $package = "$basePackage$packageSuffix"
  $url = "https://msmobilecenter.pkgs.visualstudio.com/_apis/packaging/feeds/$env:NUGET_FEED_ID/nuget/packages/$package/versions/$version/content"
  $password = [System.Text.Encoding]::UTF8.GetBytes("appcenter:$NugetPassword")
  $password = [System.Convert]::ToBase64String($password)
  $webClient = New-Object System.Net.WebClient
  $webClient.Headers.Add("Authorization","Basic $password")
  $webClient.DownloadFile($url, "$pwd/$package.$version.nupkg")
}
