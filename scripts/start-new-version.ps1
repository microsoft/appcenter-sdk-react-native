# Note: Run this from within the root directory

[CmdletBinding()]
Param(
    [string]$Version
)

.\build.ps1 -Script "version.cake" -Target "StartNewVersion" -ExtraArgs '-NewVersion="$Version"'
