#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/DemoApp/.npmrc

# Delete everything except DemoApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../TestApp ../TestAppTypescript

echo 'Create local.properties file if it does not exist'
touch -a android/local.properties
./put-azure-credentials.zh $USER_ACCOUNT $ACCESS_TOKEN

echo 'Add private cocoapods repository'
pod repo add $REPO_NAME https://$USER_ACCOUNT:$ACCESS_TOKEN@$PRIVATE_REPO_BASE_URL/$REPO_NAME
pod repo update
cd ios && pod install

# Remove unnessesary NDKs, this clean is needed to avoid failures on AppCenter CI which requires only NDK21. 
echo 'Clean unnessesary NDKs'
cd $NDK_PATH

# Enable extended globbing for using regular expression in rm command.
shopt -s extglob
rm -rf !(21.4.7075529)

echo 'NDKs after clean:'
ls $NDK_PATH