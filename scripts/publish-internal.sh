#!/bin/bash
set -e

for packageFile in output/*.nupkg
do
    curl -X PUT "https://mseng.pkgs.visualstudio.com/_packaging/150e39b1-bf52-4fdd-bc32-28d950a14304/nuget/v2/" -H "Content-Type: multipart/form-data" -H "X-NuGet-ApiKey: $NUGET_PASSWORD" -H "Content-Disposition: form-data; name=package; filename=package" -F "file=@$packageFile" -s --user $NUGET_USER:$NUGET_PASSWORD
done
