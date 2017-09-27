#!/bin/bash
# Bump version of Mobile Center React Native SDK for release

set -e

while [ "$1" != "" ]; do
    PARAM=`echo $1 | awk -F= '{print $1}'`
    VALUE=`echo $1 | awk -F= '{print $2}'`
    case $PARAM in
        --newWrapperSdkVersion)
            newWrapperSdkVersion=$VALUE ;;
        *)
    esac
    shift
done

# Exit if newWrapperSdkVersion has not been set
if [ -z $newWrapperSdkVersion ]; then
    echo "--newWrapperSdkVersion cannot be empty. Please pass in new sdk version as parameter."
    exit 1
fi

# Find out the old wrapper sdk version
oldWrapperSdkVersionString=$(grep versionName ./mobile-center/android/build.gradle)
[[ ${oldWrapperSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldWrapperSdkVersion="${BASH_REMATCH[1]}"

# Find out the old Android versionCode
oldAndroidVersionCodeString=$(grep versionCode ./mobile-center/android/build.gradle)
[[ ${oldAndroidVersionCodeString} =~ ([0-9]+) ]]
oldAndroidVersionCode="${BASH_REMATCH[1]}"

# Compute the new Android versionCode by adding one to old Android versionCode
newAndroidVersionCode=$(($oldAndroidVersionCode + 1))

# Exit if any of the parameters have not been set
if [ -z $oldWrapperSdkVersion ]; then
    echo "oldWrapperSdkVersion cannot be empty"
    exit 1
fi
if [ -z $oldAndroidVersionCode ]; then
    echo "oldAndroidVersionCode cannot be empty"
    exit 1
fi
if [ -z $newAndroidVersionCode ]; then
    echo "newAndroidVersionCode cannot be empty"
    exit 1
fi

echo "React-Native SDK $oldWrapperSdkVersion will be updated to $newWrapperSdkVersion"
echo "React-Native SDK Android VersionCode $oldAndroidVersionCode will be updated to $newAndroidVersionCode"

# Update wrapper sdk version in package.json for mobile-center, mobile-center-crashes, mobile-center-analytics, 
# mobile-center-push and mobile-center-link-script NPM packages

export newVersion=$newWrapperSdkVersion
cat ./mobile-center/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."mobile-center-link-scripts" = env.newVersion' > ./mobile-center/package.json.temp && mv ./mobile-center/package.json.temp ./mobile-center/package.json
cat ./mobile-center-crashes/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."mobile-center" = env.newVersion' > ./mobile-center-crashes/package.json.temp && mv ./mobile-center-crashes/package.json.temp ./mobile-center-crashes/package.json 
cat ./mobile-center-analytics/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."mobile-center" = env.newVersion' > ./mobile-center-analytics/package.json.temp && mv ./mobile-center-analytics/package.json.temp ./mobile-center-analytics/package.json
cat ./mobile-center-push/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."mobile-center" = env.newVersion' > ./mobile-center-push/package.json.temp && mv ./mobile-center-push/package.json.temp ./mobile-center-push/package.json
cat ./mobile-center-link-scripts/package.json | jq -r '.version = env.newVersion' > ./mobile-center-link-scripts/package.json.temp && mv ./mobile-center-link-scripts/package.json.temp ./mobile-center-link-scripts/package.json

# Update wrapperk sdk version and android VersionCode in Android build.gradle for mobile-center, mobile-center-crashes, mobile-center-analytics,
# mobile-center-push and RNMobileCenterShared projects

gradleFileContent="$(cat ./mobile-center/android/build.gradle)"
gradleFileContent=`echo "${gradleFileContent/versionName \"$oldWrapperSdkVersion\"/versionName \"$newWrapperSdkVersion\"}"`
gradleFileContent=`echo "${gradleFileContent/com.microsoft.azure.mobile.react\:mobile-center-react-native\:$oldWrapperSdkVersion/com.microsoft.azure.mobile.react:mobile-center-react-native:$newWrapperSdkVersion}"`
echo "${gradleFileContent/versionCode $oldAndroidVersionCode/versionCode $newAndroidVersionCode}" > ./mobile-center/android/build.gradle

gradleFileContent="$(cat ./mobile-center-crashes/android/build.gradle)"
gradleFileContent=`echo "${gradleFileContent/versionName \"$oldWrapperSdkVersion\"/versionName \"$newWrapperSdkVersion\"}"`
gradleFileContent=`echo "${gradleFileContent/com.microsoft.azure.mobile.react\:mobile-center-react-native\:$oldWrapperSdkVersion/com.microsoft.azure.mobile.react:mobile-center-react-native:$newWrapperSdkVersion}"`
echo "${gradleFileContent/versionCode $oldAndroidVersionCode/versionCode $newAndroidVersionCode}" > ./mobile-center-crashes/android/build.gradle

gradleFileContent="$(cat ./mobile-center-analytics/android/build.gradle)"
gradleFileContent=`echo "${gradleFileContent/versionName \"$oldWrapperSdkVersion\"/versionName \"$newWrapperSdkVersion\"}"`
gradleFileContent=`echo "${gradleFileContent/com.microsoft.azure.mobile.react\:mobile-center-react-native\:$oldWrapperSdkVersion/com.microsoft.azure.mobile.react:mobile-center-react-native:$newWrapperSdkVersion}"`
echo "${gradleFileContent/versionCode $oldAndroidVersionCode/versionCode $newAndroidVersionCode}" > ./mobile-center-analytics/android/build.gradle

gradleFileContent="$(cat ./mobile-center-push/android/build.gradle)"
gradleFileContent=`echo "${gradleFileContent/versionName \"$oldWrapperSdkVersion\"/versionName \"$newWrapperSdkVersion\"}"`
gradleFileContent=`echo "${gradleFileContent/com.microsoft.azure.mobile.react\:mobile-center-react-native\:$oldWrapperSdkVersion/com.microsoft.azure.mobile.react:mobile-center-react-native:$newWrapperSdkVersion}"`
echo "${gradleFileContent/versionCode $oldAndroidVersionCode/versionCode $newAndroidVersionCode}" > ./mobile-center-push/android/build.gradle

gradleFileContent="$(cat ./RNMobileCenterShared/android/build.gradle)"
gradleFileContent=`echo "${gradleFileContent/versionName \"$oldWrapperSdkVersion\"/versionName \"$newWrapperSdkVersion\"}"`
echo "${gradleFileContent/versionCode $oldAndroidVersionCode/versionCode $newAndroidVersionCode}" > ./RNMobileCenterShared/android/build.gradle

# Update wrapper sdk version in postlink.js for mobile-center, mobile-center-crashes, mobile-center-analytics,
# and mobile-center-push
postlinkFileContent="$(cat ./mobile-center/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./mobile-center/scripts/postlink.js

postlinkFileContent="$(cat ./mobile-center-crashes/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./mobile-center-crashes/scripts/postlink.js

postlinkFileContent="$(cat ./mobile-center-analytics/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./mobile-center-analytics/scripts/postlink.js

postlinkFileContent="$(cat ./mobile-center-push/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./mobile-center-push/scripts/postlink.js

# Update wrapper sdk version in RNMobileCenterShared/Products/RNMobileCenterShared.podspec
podspecFileContent="$(cat ./RNMobileCenterShared/Products/RNMobileCenterShared.podspec)"
echo "${podspecFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./RNMobileCenterShared/Products/RNMobileCenterShared.podspec

# Update wrapper sdk version in RNMobileCenterShared/ios/RNMobileCenterShared/RNMobileCenterShared.m
fileContent="$(cat ./RNMobileCenterShared/ios/RNMobileCenterShared/RNMobileCenterShared.m)"
echo "${fileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./RNMobileCenterShared/ios/RNMobileCenterShared/RNMobileCenterShared.m

echo "done."