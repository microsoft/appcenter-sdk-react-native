#!/bin/bash

# Define directory and file locations
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
UITEST_BUILD_DIR="$SCRIPT_DIR/../Tests/UITests/bin/Release"
BUILD_SCRIPT="build.sh"
CLEAN_TARGET="clean"

# Set default values for running locally
TEST_APK="$SCRIPT_DIR/../Tests/Droid/bin/Release/com.contoso.contoso_forms_test.apk"
TEST_IPA="$SCRIPT_DIR/../Tests/iOS/bin/iPhone/Release/Contoso.Forms.Test.iOS.ipa"
BUILD_TARGET="TestApps"

# If script is running in bitrise environment, use arguments
if ! [ -z ${IN_BITRISE+x} ]; then # We are in bitrise environment
	if [ -z ${3+x} ]; then # If there are not three arguments, exit failure
		echo "Error - usage: ./run-ui-tests.sh {PATH_TO_APK} {PATH_TO_IPA} {BUILD_TARGET}"
		exit 1
	fi
	TEST_APK=$1
	TEST_IPA=$2
	BUILD_TARGET=$3
fi

# Define test parameters
LOCALE="en-US"
USERNAME="$MOBILE_CENTER_USERNAME" # 'MOBILE_CENTER_USERNAME' environment variable must be set
IOS_DEVICES=8551ba4e # just one device. For a suite of 40, use 118f9d2f
ANDROID_DEVICES=f0b8289c # just one device. For a suite of 40, use f47808f1
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
ANDROID_APP="$MOBILE_CENTER_TEST_APP_USERNAME/$ANDROID_APP_NAME"
IOS_APP="$MOBILE_CENTER_TEST_APP_USERNAME/$IOS_APP_NAME"
TEST_SERIES="master"

# Define results constants
ANDROID_PORTAL_URL="https://mobile.azure.com/users/$MOBILE_CENTER_TEST_APP_USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://mobile.azure.com/users/$MOBILE_CENTER_TEST_APP_USERNAME/apps/$IOS_APP_NAME/test/runs/"
ANDROID_INFORMATION_FILE="android_info.txt"
IOS_INFORMATION_FILE="ios_info.txt"
ANDROID_PLATFORM_NAME="Android"
IOS_PLATFORM_NAME="iOS"

# If the MOBILE_CENTER_ANDROID_DEVICES environment variable is set, use it as the ANDROID_DEVICES
if ! [ -z ${MOBILE_CENTER_ANDROID_DEVICES+x} ]; then
	ANDROID_DEVICES="$MOBILE_CENTER_ANDROID_DEVICES"
fi

# If the MOBILE_CENTER_IOS_DEVICES environment variable is set, use it as the IOS_DEVICES
if ! [ -z ${MOBILE_CENTER_IOS_DEVICES+x} ]; then
	IOS_DEVICES="$MOBILE_CENTER_IOS_DEVICES"
fi

# Define functions

# This function extracts the test run ID from an information file, and then echoes it
# Usage: get_test_run_id {INFORMATION_FILE}
get_test_run_id() {
	INFORMATION_FILE="$1"
	while read -r line
		do
	if [ $(expr "$line" : "Test run id: ") -ne 0 ]; then
		echo $(echo $line | cut -d'"' -f 2)
		break
	fi
	done < $INFORMATION_FILE
}

# This function prints the results of test initialization
# Usage: print_initialization_results {RETURN_CODE} {PLATFORM_NAME} {PORTAL_URL} {TEST_RUN_ID}
print_initialization_results() {
	RETURN_CODE=$1
	PLATFORM_NAME="$2"
	PORTAL_URL="$3"
	TEST_RUN_ID="$4"
	if [ $RETURN_CODE -ne 0 ]; then
		echo "$PLATFORM_NAME test failed to initiate."
	fi
	if [ $RETURN_CODE -eq 0 ]; then
		echo "$PLATFORM_NAME test run ID: $TEST_RUN_ID"
		echo "$PLATFORM_NAME test results: $PORTAL_URL$TEST_RUN_ID"
	fi
}

# This function initializes tests for the given parameters
# Usage: initialize_tests {APP_NAME} {DEVICES_CODE} {APP_PACKAGE} {INFORMATION_FILE}
initialize_tests() {
	APP_NAME="$1"
	DEVICES_CODE="$2"
	APP_PACKAGE="$3"
	INFORMATION_FILE="$4"
	mobile-center test run uitest --app $APP_NAME\
 	--devices $DEVICES_CODE --app-path $APP_PACKAGE\
  	--test-series $TEST_SERIES --locale $LOCALE\
  	--build-dir $UITEST_BUILD_DIR --async true > $INFORMATION_FILE
	echo $?
}

# The MOBILE_CENTER_TEST_APP_USERNAME environment variable must be set
if [ -z ${MOBILE_CENTER_TEST_APP_USERNAME+x} ]; then
	echo "Error - the environment variable MOBILE_CENTER_TEST_APP_USERNAME must be set."
	exit 1
fi

# Log in to mobile center
./mobile-center-login.sh
if [ $? -ne 0 ]; then
	exit 1
fi

# Build tests
pushd ..
echo "Cleaning..."
sh $BUILD_SCRIPT -t $CLEAN_TARGET # clean so that we don't accidentally update to snapshot
if [ $? -ne 0 ]; then
    echo "An error occured while cleaning."
    popd
    exit 1
fi
echo "Building target \"$BUILD_TARGET\"..."
sh $BUILD_SCRIPT -t $BUILD_TARGET
if [ $? -ne 0 ]; then
    echo "An error occured while building tests."
    popd
    exit 1
fi
popd

# Run Android tests
echo "Initializing Android tests..."
ANDROID_RETURN_CODE=$(initialize_tests $ANDROID_APP $ANDROID_DEVICES $TEST_APK $ANDROID_INFORMATION_FILE)
ANDROID_TEST_RUN_ID=$(get_test_run_id $ANDROID_INFORMATION_FILE)
print_initialization_results $ANDROID_RETURN_CODE "$ANDROID_PLATFORM_NAME" "$ANDROID_PORTAL_URL" "$ANDROID_TEST_RUN_ID"
rm $ANDROID_INFORMATION_FILE

# Run iOS tests
echo "Initializing iOS tests..."
IOS_RETURN_CODE=$(initialize_tests $IOS_APP $IOS_DEVICES $TEST_IPA $IOS_INFORMATION_FILE)
IOS_TEST_RUN_ID=$(get_test_run_id $IOS_INFORMATION_FILE)
print_initialization_results $IOS_RETURN_CODE "$IOS_PLATFORM_NAME" "$IOS_PORTAL_URL" "$IOS_TEST_RUN_ID"
rm $IOS_INFORMATION_FILE

# If iOS or Android tests failed to be initiated, exit failure. Otherwise exit success
if [ $IOS_RETURN_CODE -ne 0 ] || [ $ANDROID_RETURN_CODE -ne 0 ]; then	
	exit 1
fi

# If script is running in bitrise environment, upload test run IDs to Azure Storage
if ! [ -z ${IN_BITRISE+x} ]; then # Then we are in bitrise environment
	echo "Writing test run IDs to files..."
	echo "$IOS_TEST_RUN_ID" > $IOS_TEST_RUN_ID_FILE
	echo "$ANDROID_TEST_RUN_ID" > $ANDROID_TEST_RUN_ID_FILE
	azure storage blob upload -q $IOS_TEST_RUN_ID_FILE $AZURE_STORAGE_CONTAINER
	azure storage blob upload -q $ANDROID_TEST_RUN_ID_FILE $AZURE_STORAGE_CONTAINER
fi

exit 0
