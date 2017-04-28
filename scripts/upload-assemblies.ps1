# Note: Run this from within the root directory

[CmdletBinding()]
Param(
    [string]$StorageId
)

.\build.ps1 -Target "UploadAssemblies" -ExtraArgs '-StorageId="$StorageId"'
