#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $GOOGLE_SERVICES_JSON | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/TestApp/android/app/google-services.json
./update-npm-packages.sh
# Delete everything except TestApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../DemoApp
