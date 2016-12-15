#!/bin/bash

if ! [ -z ${1+x} ]; then
	if [ -z ${2+x} ] || [ -z ${3+x} ]; then
		echo "Error - usage: ./run-ui-tests.sh {PATH_TO_APK} {PATH_TO_IPA} {BUILD_TARGET}"
	fi
fi

# Define directory and file locations
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
UITEST_BUILD_DIR=$SCRIPT_DIR/../Tests/UITests/bin/Release
BUILD_SCRIPT=build.sh
TEST_APK=$1
TEST_IPA=$2
BUILD_TARGET=$3
CLEAN_TARGET="clean"
FROM_BITRISE=0
# If there are no arguments, use default values
if [ -z ${1+x} ]; then
	TEST_APK=$SCRIPT_DIR/../Tests/Droid/bin/Release/com.contoso.contoso_forms_test.apk
	TEST_IPA=$SCRIPT_DIR/../Tests/iOS/bin/iPhone/Release/Contoso.Forms.Test.iOS.ipa
	BUILD_TARGET=TestApps
fi

# Need to know whether we are on bitrise for environment variables
# This is indicated by a fourth argument - if there is one, we are on bitrise
if ! [ -z ${4+x} ]; then
	FROM_BITRISE=1
fi

# Define test parameters
LOCALE="en-US"
USERNAME="$MOBILE_CENTER_USERNAME" # 'MOBILE_CENTER_USERNAME' environment variable must be set
IOS_DEVICES=8551ba4e # just one device. For a suite of 40, use 118f9d2f
ANDROID_DEVICES=f0b8289c # just one device. For a suite of 40, use f47808f1
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
ANDROID_APP="$USERNAME/$ANDROID_APP_NAME"
IOS_APP="$USERNAME/$IOS_APP_NAME"
TEST_SERIES="master"

# Define results constants
ANDROID_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$IOS_APP_NAME/test/runs/"
ANDROID_INFORMATION_FILE="android_info.txt"
IOS_INFORMATION_FILE="ios_info.txt"

# Log in to mobile center
./mobile-center-login.sh
if [ $? -ne 0 ]; then
	exit 1
fi

# Build tests
echo "Building target \"$BUILD_TARGET\"..."

pushd ..
sh $BUILD_SCRIPT -t $CLEAN_TARGET # clean so that we don't accidentally update to snapshot
sh $BUILD_SCRIPT -t $BUILD_TARGET
if [ $? -ne 0 ]; then
    echo "An error occured while building tests."
    popd
    exit 1
fi
popd

# Run Android tests
echo "Initiating Android tests..."
mobile-center test run uitest --app $ANDROID_APP\
 --devices $ANDROID_DEVICES --app-path $TEST_APK\
  --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_BUILD_DIR --async true > $ANDROID_INFORMATION_FILE
ANDROID_RETURN_CODE=$?
ANDROID_TEST_RUN_ID=$(
while read -r line
do
	if [ $(expr "$line" : "Test run id: ") -ne 0 ]; then
		echo $(echo $line | cut -d'"' -f 2)
		break
	fi
done < $ANDROID_INFORMATION_FILE)
rm $ANDROID_INFORMATION_FILE

# Print results of Android test initiation
if [ $ANDROID_RETURN_CODE -ne 0 ]; then
	echo "Android test failed to initiate."
fi
if [ $ANDROID_RETURN_CODE -eq 0 ]; then
	echo "Android test run id: $ANDROID_TEST_RUN_ID"
	echo "Android test results: $ANDROID_PORTAL_URL$ANDROID_TEST_RUN_ID"
fi

# Run iOS tests
echo "Initiating iOS tests..."
mobile-center test run uitest --app $IOS_APP\
   --devices $IOS_DEVICES --app-path $TEST_IPA\
   --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_BUILD_DIR --async true > $IOS_INFORMATION_FILE
IOS_RETURN_CODE=$?
IOS_TEST_RUN_ID=$(
while read -r line
do
   	if [ $(expr "$line" : "Test run id: ") -ne 0 ]; then
		echo $(echo $line | cut -d'"' -f 2)
		break
	fi
done < $IOS_INFORMATION_FILE)
rm $IOS_INFORMATION_FILE

# Print results of iOS test initiation
if [ $IOS_RETURN_CODE -ne 0 ]; then
	echo "iOS test failed to initiate."
fi
if [ $IOS_RETURN_CODE -eq 0 ]; then
	echo "iOS test run id: $IOS_TEST_RUN_ID"
	echo "iOS test results: $IOS_PORTAL_URL$ANDROID_TEST_RUN_ID"
fi

# If iOS or Android tests failed to be initiated, exit failure. Otherwise exit success
if [ $IOS_RETURN_CODE -ne 0 ] || [ $ANDROID_RETURN_CODE -ne 0 ]; then	
	exit 1
fi

if [ $FROM_BITRISE -eq 1 ]; then
	envman add --key ANDROID_TEST_RUN_ID_ENV --value "$ANDROID_TEST_RUN_ID"
	envman add --key IOS_TEST_RUN_ID_ENV --value "$IOS_TEST_RUN_ID"
fi

exit 0
