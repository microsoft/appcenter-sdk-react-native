#!/bin/bash
# Replace app-secret's and tokens values in test apps. Update TestApp by default.

# Usage:
# ./scripts/update-app-secrets.sh

# Dynamic constants.
appName="TestApp"
appPrefix="INT"

# Constants for iOS and Android.
declare -a platformConstants=(
    "RN_IOS_${appPrefix}"
    "RN_IOS_TARGET_TOKEN_${appPrefix}"
    "RN_IOS_PARENT_TARGET_TOKEN_${appPrefix}"
    "RN_IOS_CHILD1_TARGET_TOKEN_${appPrefix}"
    "RN_IOS_CHILD2_TARGET_TOKEN_${appPrefix}"

    "RN_ANDROID_${appPrefix}"
    "RN_ANDROID_TARGET_TOKEN_${appPrefix}"
    "RN_ANDROID_PARENT_TARGET_TOKEN_${appPrefix}"
    "RN_ANDROID_CHILD1_TARGET_TOKEN_${appPrefix}"
    "RN_ANDROID_CHILD2_TARGET_TOKEN_${appPrefix}")

# Files which should be changed.
declare -a targetFiles=("${appName}/app/Constants.android.js"
    "${appName}/app/Constants.ios.js"
    "${appName}/app/screens/AppCenterScreen.js"
    "${appName}/ios/${appName}/AppCenter-Config.plist"
    "${appName}/ios/${appName}/Info.plist"
    "${appName}/android/app/src/main/assets/appcenter-config.json"
    "${appName}/android/app/src/main/AndroidManifest.xml")

# Print info about current job.
echo "Insert secrets to ${appName} app."

# Update files from array.
for constant in "${platformConstants[@]}"
do
    for file in "${targetFiles[@]}"
    do
        # Replace secret value from enviroment variables.
        sed -i '' "s/{$constant}/"${!constant}"/g" $file
    done
done
