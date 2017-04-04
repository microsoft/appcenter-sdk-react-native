# Note: Run this from within the root directory

[CmdletBinding()]
Param(
    [string]$DemoVersion
)

.\build.ps1 -Script "version.cake" -Target "UpdateDemoVersion" -ExtraArgs '-DemoVersion="$DemoVersion"'
