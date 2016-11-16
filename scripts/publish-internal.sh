#!/bin/bash
set -e

for packageFile in output/*.nupkg
do
    curl -X PUT "https://mseng.pkgs.visualstudio.com/_packaging/bbab086b-5df4-46c8-85ef-e614c7c364ad/nuget/v2/" -H "Content-Type: multipart/form-data" -H "X-NuGet-ApiKey: $NUGET_PASSWORD" -H "Content-Disposition: form-data; name=package; filename=package" -F "file=@$packageFile" -s --user $NUGET_USER:$NUGET_PASSWORD
done
