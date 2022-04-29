#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
echo $NPM_RC | base64 --decode > $APPCENTER_SOURCE_DIRECTORY/DemoApp/.npmrc
# Delete everything except DemoApp folder
rm -rf ../appcenter* ../AppCenterReactNativeShared ../TestApp34 ../BrownfieldTestApp ../TestApp ../TestAppTypescript

echo 'Create local.properties file if it does not exist'
touch -a android/local.properties
./put-azure-credentials.zh $USER_ACCOUNT $ACCESS_TOKEN

echo 'Setting AppSecrets'
cd .. && ./scripts/update-app-secrets.sh PROD && cd DemoApp

echo 'Add private cocoapods repository'
pod repo add $REPO_NAME https://$USER_ACCOUNT:$ACCESS_TOKEN@$PRIVATE_REPO_BASE_URL/$REPO_NAME
pod repo update
cd ios && pod install

# Remove all NDKs except NDK 21 to avoid failures on App Center CI with error:
# > No toolchains found in the NDK toolchains folder for ABI with prefix: arm-linux-androideabi 
echo 'Clean unnessesary NDKs'
cd $NDK_PATH

# Enable extended globbing for using regular expression in rm command.
shopt -s extglob
rm -rf !($NDK_VERSION_FOR_BUILD)

echo 'NDKs after clean:'
ls $NDK_PATH