#!/bin/bash
# Update App Center React Native SDK to reference a new version of App Center iOS SDK

set -e

while [ "$1" != "" ]; do
    PARAM=`echo $1 | awk -F= '{print $1}'`
    VALUE=`echo $1 | awk -F= '{print $2}'`
    case $PARAM in
        --newiOSSdkVersion)
            newiOSSdkVersion=$VALUE ;;
        *)
    esac
    shift
done

# Exit if newiOSSdkVersion has not been set
if [ -z $newiOSSdkVersion ]; then
    echo "--newiOSSdkVersion cannot be empty. Please pass in new iOS sdk version as parameter."
    exit 1
fi

# Find out the old iOS sdk version
oldiOSSdkVersionString=$(grep AppCenter/Core ./AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec)
[[ ${oldiOSSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldiOSSdkVersion="${BASH_REMATCH[1]}"

# Exit if old iOS sdk version is not set
if [ -z $oldiOSSdkVersion ]; then
    echo "oldiOSSdkVersion cannot be empty"
    exit 1
fi

echo "React-Native iOS version $oldiOSSdkVersion will be updated to $newiOSSdkVersion"

# Update iOS sdk version in postlink.js for appcenter-crashes, appcenter-analytics,
# appcenter-auth appcenter-push projects
fileContent="$(cat ./appcenter-crashes/scripts/postlink.js)"
echo "${fileContent/\'AppCenter\/Crashes\', version\: \'$oldiOSSdkVersion\'/'AppCenter/Crashes', version: '$newiOSSdkVersion'}" > ./appcenter-crashes/scripts/postlink.js

fileContent="$(cat ./appcenter-analytics/scripts/postlink.js)"
echo "${fileContent/\'AppCenter\/Analytics\', version\: \'$oldiOSSdkVersion\'/'AppCenter/Analytics', version: '$newiOSSdkVersion'}" > ./appcenter-analytics/scripts/postlink.js

fileContent="$(cat ./appcenter-auth/scripts/postlink.js)"
echo "${fileContent/\'AppCenter\/Auth\', version\: \'$oldiOSSdkVersion\'/'AppCenter/Auth', version: '$newiOSSdkVersion'}" > ./appcenter-auth/scripts/postlink.js

fileContent="$(cat ./appcenter-push/scripts/postlink.js)"
echo "${fileContent/\'AppCenter\/Push\', version\: \'$oldiOSSdkVersion\'/'AppCenter/Push', version: '$newiOSSdkVersion'}" > ./appcenter-push/scripts/postlink.js

# Update iOS sdk version in AppCenterReactNativeShared podspec
fileContent="$(cat ./AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec)"
echo "${fileContent/s.dependency \'AppCenter\/Core\', \'$oldiOSSdkVersion\'/s.dependency 'AppCenter/Core', '$newiOSSdkVersion'}" > ./AppCenterReactNativeShared/Products/AppCenterReactNativeShared.podspec

# Update wrapper sdk version in local.podspec,
# so that local.podspec is in sync with AppCenterReactNativeShared.podspec.
fileContent="$(cat ./AppCenterReactNativeShared/Products/local.podspec)"
echo "${fileContent/s.dependency \'AppCenter\/Core\', \'$oldiOSSdkVersion\'/s.dependency 'AppCenter/Core', '$newiOSSdkVersion'}" > ./AppCenterReactNativeShared/Products/local.podspec

echo "done."
