#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/DemoApp/.npmrc
# Delete everything except DemoApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../TestApp

sudo xcode-select -r
cd ~/.cocoapods/repos
git clone https://$USER_ACCOUNT:$ACCESS_TOKEN@msmobilecenter.visualstudio.com/SDK/_git/$REPO_NAME
# There is a bug when trying to do a pod repo add from build agent
# pod repo add $REPO_NAME https://$USER_ACCOUNT:$ACCESS_TOKEN@msmobilecenter.visualstudio.com/SDK/_git/$REPO_NAME --verbose
cd $BUILD_REPOSITORY_LOCALPATH
cd DemoApp/ios
pod install
