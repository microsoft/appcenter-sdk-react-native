#!/bin/bash
cd Products
podspecFile="${podspecFile:-AppCenterReactNativeShared.podspec}"
wrapperSdkString=$(grep s.version ${podspecFile})
[[ ${wrapperSdkString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
wrapperSdkVersion="${BASH_REMATCH[1]}"
zipfilename="AppCenter-SDK-ReactNative-iOS-Pod-${wrapperSdkVersion}.zip"
if [ -f $zipfilename ] ; then
    rm $zipfilename
    echo "  removed old zip"
fi
cp ../LICENSE AppCenterReactNativeShared
zip -r $zipfilename AppCenterReactNativeShared
echo "output is here: Products/$zipfilename"
