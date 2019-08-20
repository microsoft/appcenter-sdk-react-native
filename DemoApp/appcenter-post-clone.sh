#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/DemoApp/.npmrc
# Delete everything except DemoApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../TestApp

pod repo add AppCenterSDK-Specs-Private https://msmobilecenter.visualstudio.com/SDK/_git/AppCenterSDK-Specs-Private
cd ios && pod install
