#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/DemoApp/.npmrc
# Delete everything except DemoApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../TestApp

pod repo add $REPO_NAME https://$USER_ACCOUNT:$ACCESS_TOKEN@$PRIVATE_REPO_BASE_URL/$REPO_NAME
pod repo update
cd ios && pod install
