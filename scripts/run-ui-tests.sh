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

# The APP_CENTER_USERNAME environment variable must be set
if [ -z ${APP_CENTER_USERNAME+x} ]; then
    echo "Error - the environment variable APP_CENTER_USERNAME must be set."
    exit 1
fi

# Define test parameters
LOCALE="en-US"
# For a larger suite, go to portal, pretend to start a test suite, select devices, click next until you see CLI instructions and copy the hash code
IOS_DEVICES=8551ba4e # just one device.
ANDROID_DEVICES=f0b8289c # just one device.
ANDROID_APP_NAME="appcenter-xamarin-testing-app-android"
IOS_APP_NAME="appcenter-xamarin-testing-app-ios"
ANDROID_APP="$APP_CENTER_USERNAME/$ANDROID_APP_NAME"
IOS_APP="$APP_CENTER_USERNAME/$IOS_APP_NAME"
TEST_SERIES="master"

# Define results constants
ANDROID_PORTAL_URL="https://appcenter.ms/users/$APP_CENTER_USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://appcenter.ms/users/$APP_CENTER_USERNAME/apps/$IOS_APP_NAME/test/runs/"
ANDROID_INFORMATION_FILE="android_info.txt"
IOS_INFORMATION_FILE="ios_info.txt"
ANDROID_PLATFORM_NAME="Android"
IOS_PLATFORM_NAME="iOS"

# If the APP_CENTER_ANDROID_DEVICES environment variable is set, use it as the ANDROID_DEVICES
if ! [ -z ${APP_CENTER_ANDROID_DEVICES+x} ]; then
    ANDROID_DEVICES="$APP_CENTER_ANDROID_DEVICES"
fi

# If the APP_CENTER_IOS_DEVICES environment variable is set, use it as the IOS_DEVICES
if ! [ -z ${APP_CENTER_IOS_DEVICES+x} ]; then
    IOS_DEVICES="$APP_CENTER_IOS_DEVICES"
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

# Log in to app center
./appcenter-login.sh
if [ $? -ne 0 ]; then
    exit 1
fi

# Build tests
pushd ..
echo "Cleaning..."
sh $BUILD_SCRIPT -t=$CLEAN_TARGET # clean so that we don't accidentally update to snapshot
if [ $? -ne 0 ]; then
    echo "An error occured while cleaning."
    popd
    exit 1
fi
echo "Building target \"$BUILD_TARGET\"..."
sh $BUILD_SCRIPT -t=$BUILD_TARGET
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
