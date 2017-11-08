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
# appcenter-push and AppCenterReactNativeShared projects
gradleFileContent="$(cat ./appcenter/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.appcenter\:appcenter\:$oldAndroidSdkVersion/com.microsoft.appcenter:appcenter:$newAndroidSdkVersion}" > ./appcenter/android/build.gradle

gradleFileContent="$(cat ./appcenter-crashes/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.appcenter\:appcenter-crashes\:$oldAndroidSdkVersion/com.microsoft.appcenter:appcenter-crashes:$newAndroidSdkVersion}" > ./appcenter-crashes/android/build.gradle

gradleFileContent="$(cat ./appcenter-analytics/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.appcenter\:appcenter-analytics\:$oldAndroidSdkVersion/com.microsoft.appcenter:appcenter-analytics:$newAndroidSdkVersion}" > ./appcenter-analytics/android/build.gradle

gradleFileContent="$(cat ./appcenter-push/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.appcenter\:appcenter-push\:$oldAndroidSdkVersion/com.microsoft.appcenter:appcenter-push:$newAndroidSdkVersion}" > ./appcenter-push/android/build.gradle

gradleFileContent="$(cat ./AppCenterReactNativeShared/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.appcenter\:appcenter\:$oldAndroidSdkVersion/com.microsoft.appcenter:appcenter:$newAndroidSdkVersion}" > ./AppCenterReactNativeShared/android/build.gradle

echo "done."
