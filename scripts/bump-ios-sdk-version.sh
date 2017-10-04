#!/bin/bash
# Update Mobile Center React Native SDK to reference a new version of Mobile Center iOS SDK

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
oldiOSSdkVersionString=$(grep MobileCenter/Core ./RNMobileCenterShared/Products/RNMobileCenterShared.podspec)
[[ ${oldiOSSdkVersionString} =~ ([0-9]+.[0-9]+.[0-9]+) ]]
oldiOSSdkVersion="${BASH_REMATCH[1]}"

# Exit if old iOS sdk version is not set
if [ -z $oldiOSSdkVersion ]; then
    echo "oldiOSSdkVersion cannot be empty"
    exit 1
fi

echo "React-Native iOS version $oldiOSSdkVersion will be updated to $newiOSSdkVersion"

# Update iOS sdk version in postlink.js for mobile-center-crashes, mobile-center-analytics,
# mobile-center-push projects
fileContent="$(cat ./mobile-center-crashes/scripts/postlink.js)"
echo "${fileContent/\'MobileCenter\/Crashes\', version\: \'$oldiOSSdkVersion\'/'MobileCenter/Crashes', version: '$newiOSSdkVersion'}" > ./mobile-center-crashes/scripts/postlink.js

fileContent="$(cat ./mobile-center-analytics/scripts/postlink.js)"
echo "${fileContent/\'MobileCenter\/Analytics\', version\: \'$oldiOSSdkVersion\'/'MobileCenter/Analytics', version: '$newiOSSdkVersion'}" > ./mobile-center-analytics/scripts/postlink.js

fileContent="$(cat ./mobile-center-push/scripts/postlink.js)"
echo "${fileContent/\'MobileCenter\/Push\', version\: \'$oldiOSSdkVersion\'/'MobileCenter/Push', version: '$newiOSSdkVersion'}" > ./mobile-center-push/scripts/postlink.js

# Update iOS sdk version in RNMobileCenterShared podspec
fileContent="$(cat ./RNMobileCenterShared/Products/RNMobileCenterShared.podspec)"
echo "${fileContent/s.dependency \'MobileCenter\/Core\', \'~> $oldiOSSdkVersion\'/s.dependency 'MobileCenter/Core', '~> $newiOSSdkVersion'}" > ./RNMobileCenterShared/Products/RNMobileCenterShared.podspec

echo "done."
