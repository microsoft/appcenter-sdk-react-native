#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/TestApp/.npmrc
echo $GOOGLE_SERVICES_JSON | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/TestApp/android/app/google-services.json
./update-npm-packages.sh
# App Center React Native Android Build "Copy Files" Task will throw 
# if there's any broken symlinks inside the repo. So installing
# npm packages for BrownfieldTestApp is necessary to allow build pass.
echo "Install npm dependencies for BrownfieldTestApp"
cd BrownfieldTestApp
npm install
