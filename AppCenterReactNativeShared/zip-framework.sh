#!/bin/bash
cd Products
wrapperSdkStringn=$(grep s.version AppCenterReactNativeShared.podspec)
[[ ${wrapperSdkStringn} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
wrapperSdkVersion="${BASH_REMATCH[1]}"
zipfilename="AppCenter-SDK-ReactNative-iOS-Pod-${wrapperSdkVersion}.zip"
if [ -f $zipfilename ] ; then
    rm $zipfilename
    echo "  removed old zip"
fi
cp ../../LICENSE AppCenterReactNativeShared
zip -r $zipfilename AppCenterReactNativeShared
echo "output is here: Products/$zipfilename"
