#!/bin/bash
# Update App Center React Native SDK to reference a new version of App Center Android SDK

set -e

while [ "$1" != "" ]; do
    PARAM=`echo $1 | awk -F= '{print $1}'`
    VALUE=`echo $1 | awk -F= '{print $2}'`
    case $PARAM in
        --newAndroidSdkVersion)
            newAndroidSdkVersion=$VALUE ;;
        *)
    esac
    shift
done

# Exit if newAndroidSdkVersion has not been set
if [ -z $newAndroidSdkVersion ]; then
    echo "--newAndroidSdkVersion cannot be empty. Please pass in new android sdk version as parameter."
    exit 1
fi

# Find out the old android sdk version
oldAndroidSdkVersionString=$(grep com.microsoft.appcenter:appcenter ./appcenter/android/build.gradle)
[[ ${oldAndroidSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldAndroidSdkVersion="${BASH_REMATCH[1]}"

# Exit if old android sdk is not set
if [ -z $oldAndroidSdkVersion ]; then
    echo "oldAndroidSdkVersion cannot be empty"
    exit 1
fi

echo "React-Native Android version $oldAndroidSdkVersion will be updated to $newAndroidSdkVersion"

# Update android sdk version in build.gradle for appcenter, appcenter-crashes, appcenter-analytics,
# appcenter-auth, appcenter-push and AppCenterReactNativeShared projects
for file in \
    "appcenter/android/build.gradle" \
    "appcenter-analytics/android/build.gradle" \
    "appcenter-auth/android/build.gradle" \
    "appcenter-crashes/android/build.gradle" \
    "appcenter-push/android/build.gradle" \
    "AppCenterReactNativeShared/android/build.gradle"
do
    sed -E -i '' "s#(com\.microsoft\.appcenter:appcenter.*:)([^:'])+#\1$newAndroidSdkVersion#g" $file
done

echo "done."
