#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
pushd $APPCENTER_SOURCE_DIRECTORY
sed -i '' "s/NUGET_PASSWORD/$NUGET_PASSWORD/g" NuGet.config
./build.sh -t=externals-android
popd
