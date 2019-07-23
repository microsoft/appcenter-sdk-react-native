#!/bin/bash
# Bump version of App Center React Native SDK for release

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
oldWrapperSdkVersionString=$(grep versionName ./appcenter/android/build.gradle)
[[ ${oldWrapperSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldWrapperSdkVersion="${BASH_REMATCH[1]}"

# Find out the old Android versionCode
oldAndroidVersionCodeString=$(grep versionCode ./appcenter/android/build.gradle)
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

# Update wrapper sdk version in package.json for appcenter, appcenter-crashes, appcenter-analytics, 
# appcenter-auth, appcenter-push and appcenter-link-script NPM packages

export newVersion=$newWrapperSdkVersion
cat ./appcenter/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."appcenter-link-scripts" = env.newVersion' > ./appcenter/package.json.temp && mv ./appcenter/package.json.temp ./appcenter/package.json
cat ./appcenter-crashes/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."appcenter" = env.newVersion' > ./appcenter-crashes/package.json.temp && mv ./appcenter-crashes/package.json.temp ./appcenter-crashes/package.json 
cat ./appcenter-analytics/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."appcenter" = env.newVersion' > ./appcenter-analytics/package.json.temp && mv ./appcenter-analytics/package.json.temp ./appcenter-analytics/package.json
cat ./appcenter-auth/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."appcenter" = env.newVersion' > ./appcenter-auth/package.json.temp && mv ./appcenter-auth/package.json.temp ./appcenter-auth/package.json
cat ./appcenter-push/package.json | jq -r '.version = env.newVersion' | jq -r '.dependencies."appcenter" = env.newVersion' > ./appcenter-push/package.json.temp && mv ./appcenter-push/package.json.temp ./appcenter-push/package.json
cat ./appcenter-link-scripts/package.json | jq -r '.version = env.newVersion' > ./appcenter-link-scripts/package.json.temp && mv ./appcenter-link-scripts/package.json.temp ./appcenter-link-scripts/package.json

# Update wrapperk sdk version and android VersionCode in Android build.gradle for appcenter, appcenter-crashes, appcenter-analytics,
# appcenter-auth, appcenter-push and AppCenterReactNativeShared projects
for file in \
    "appcenter/android/build.gradle" \
    "appcenter-analytics/android/build.gradle" \
    "appcenter-auth/android/build.gradle" \
    "appcenter-crashes/android/build.gradle" \
    "appcenter-push/android/build.gradle" \
    "AppCenterReactNativeShared/android/build.gradle"
do
    sed -E -i '' "s#(com\.microsoft\.appcenter\.reactnative:appcenter-react-native:)([^:'])+#\1$newWrapperSdkVersion#g" $file
    sed -E -i '' "s#versionName '(.*)'#versionName '$newWrapperSdkVersion'#g" $file
    sed -E -i '' "s#versionCode (.*)#versionCode $newAndroidVersionCode#g" $file
    sed -E -i '' "s#[^/](api project\(':AppCenterReactNativeShared'\))# //\1#g" $file
    sed -E -i '' "s#//(api 'com\.microsoft\.appcenter\.reactnative)#\1#g" $file
done

# Update wrapper sdk version in postlink.js for appcenter, appcenter-crashes, appcenter-analytics,
# appcenter-auth, and appcenter-push
postlinkFileContent="$(cat ./appcenter/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./appcenter/scripts/postlink.js

postlinkFileContent="$(cat ./appcenter-crashes/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./appcenter-crashes/scripts/postlink.js

postlinkFileContent="$(cat ./appcenter-analytics/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./appcenter-analytics/scripts/postlink.js

postlinkFileContent="$(cat ./appcenter-auth/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./appcenter-auth/scripts/postlink.js

postlinkFileContent="$(cat ./appcenter-push/scripts/postlink.js)"
echo "${postlinkFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./appcenter-push/scripts/postlink.js

# Update wrapper sdk version in AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec
podspecFileContent="$(cat ./AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec)"
echo "${podspecFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec

# Update wrapper sdk version in AppCenterReactNativeShared/Products/local.podspec,
# so that local.podspec is in sync with AppCenterReactNativeShared.podspec.
localPodspecFileContent="$(cat ./AppCenterReactNativeShared/Products/local.podspec)"
echo "${localPodspecFileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./AppCenterReactNativeShared/Products/local.podspec

# Update wrapper sdk version in AppCenterReactNativeShared/ios/AppCenterReactNativeShared/AppCenterReactNativeShared.m
fileContent="$(cat ./AppCenterReactNativeShared/ios/AppCenterReactNativeShared/AppCenterReactNativeShared.m)"
echo "${fileContent/$oldWrapperSdkVersion/$newWrapperSdkVersion}" > ./AppCenterReactNativeShared/ios/AppCenterReactNativeShared/AppCenterReactNativeShared.m

echo "done."
