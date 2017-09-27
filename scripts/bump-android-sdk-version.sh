#!/bin/bash
# Update Mobile Center React Native SDK to reference a new version of Mobile Center Android SDK

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
oldAndroidSdkVersionString=$(grep com.microsoft.azure.mobile:mobile-center ./mobile-center/android/build.gradle)
[[ ${oldAndroidSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldAndroidSdkVersion="${BASH_REMATCH[1]}"

# Exit if old android sdk is not set
if [ -z $oldAndroidSdkVersion ]; then
    echo "oldAndroidSdkVersion cannot be empty"
    exit 1
fi

echo "React-Native Android version $oldAndroidSdkVersion will be updated to $newAndroidSdkVersion"

# Update android sdk version in build.gradle for mobile-center, mobile-center-crashes, mobile-center-analytics,
# mobile-center-push and RNMobileCenterShared projects
gradleFileContent="$(cat ./mobile-center/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.azure.mobile\:mobile-center\:$oldAndroidSdkVersion/com.microsoft.azure.mobile:mobile-center:$newAndroidSdkVersion}" > ./mobile-center/android/build.gradle

gradleFileContent="$(cat ./mobile-center-crashes/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.azure.mobile\:mobile-center-crashes\:$oldAndroidSdkVersion/com.microsoft.azure.mobile:mobile-center-crashes:$newAndroidSdkVersion}" > ./mobile-center-crashes/android/build.gradle

gradleFileContent="$(cat ./mobile-center-analytics/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.azure.mobile\:mobile-center-analytics\:$oldAndroidSdkVersion/com.microsoft.azure.mobile:mobile-center-analytics:$newAndroidSdkVersion}" > ./mobile-center-analytics/android/build.gradle

gradleFileContent="$(cat ./mobile-center-push/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.azure.mobile\:mobile-center-push\:$oldAndroidSdkVersion/com.microsoft.azure.mobile:mobile-center-push:$newAndroidSdkVersion}" > ./mobile-center-push/android/build.gradle

gradleFileContent="$(cat ./RNMobileCenterShared/android/build.gradle)"
echo "${gradleFileContent/com.microsoft.azure.mobile\:mobile-center\:$oldAndroidSdkVersion/com.microsoft.azure.mobile:mobile-center:$newAndroidSdkVersion}" > ./RNMobileCenterShared/android/build.gradle

echo "done."
