#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
sed -i '' "s/NUGET_PASSWORD/$NUGET_PASSWORD/g" $APPCENTER_SOURCE_DIRECTORY/NuGet.config
$APPCENTER_SOURCE_DIRECTORY/build.sh -t=externals-ios
