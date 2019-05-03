#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Define directory and file locations
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
UITEST_BUILD_DIR="$SCRIPT_DIR/../Tests/UITests/bin/Release"
BUILD_SCRIPT="build.sh"

# Built application files
TEST_APK="$SCRIPT_DIR/../Tests/Droid/bin/Release/com.contoso.contoso_forms_test.apk"
TEST_IPA="$SCRIPT_DIR/../Tests/iOS/bin/iPhone/Release/Contoso.Forms.Test.iOS.ipa"

# Set defaults but accept also positional parameters for the following:
APP_CENTER_USERNAME=${1:-$APP_CENTER_USERNAME}
APP_CENTER_API_TOKEN=${2:-$APP_CENTER_API_TOKEN}

# Check credentials are set
if [ -z ${APP_CENTER_USERNAME} ]; then
    echo "Error - the environment variable APP_CENTER_USERNAME must be set."
    exit 1
fi
if [ -z ${APP_CENTER_API_TOKEN} ]; then
    echo "Error - the environment variable APP_CENTER_API_TOKEN must be set."
    exit 1
fi

# Define test parameters
LOCALE="en-US"

# For a larger suite, go to portal, pretend to start a test suite, select devices, click next until you see CLI instructions and copy the hash code
IOS_DEVICES=8551ba4e # just one device.
ANDROID_DEVICES=f0b8289c # just one device.
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
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

# This function initializes tests for the given parameters
# Usage: initialize_tests {APP_NAME} {DEVICES_CODE} {APP_PACKAGE} {INFORMATION_FILE}
initialize_tests() {
    APP_NAME="$1"
    DEVICES_CODE="$2"
    APP_PACKAGE="$3"
    INFORMATION_FILE="$4"
    appcenter test run uitest --app $APP_NAME\
     --devices $DEVICES_CODE --app-path $APP_PACKAGE\
      --test-series $TEST_SERIES --locale $LOCALE\
      --build-dir $UITEST_BUILD_DIR --async true > $INFORMATION_FILE
    echo $?
}

# Log in to app center
APP_CENTER_API_TOKEN=$APP_CENTER_API_TOKEN ./appcenter-login.sh
if [ $? -ne 0 ]; then
    exit 1
fi

# Build tests
pushd ..
echo "Building target 'UITest'..."
sh $BUILD_SCRIPT -s "scripts/uitest.cake"
if [ $? -ne 0 ]; then
    echo "An error occured while building tests."
    popd
    exit 1
fi
popd

# Run Android tests
echo "Initializing Android tests..."
ANDROID_RETURN_CODE=$(initialize_tests $ANDROID_APP $ANDROID_DEVICES $TEST_APK $ANDROID_INFORMATION_FILE)
cat $ANDROID_INFORMATION_FILE
rm $ANDROID_INFORMATION_FILE

# Run iOS tests
echo "Initializing iOS tests..."
IOS_RETURN_CODE=$(initialize_tests $IOS_APP $IOS_DEVICES $TEST_IPA $IOS_INFORMATION_FILE)
cat $IOS_INFORMATION_FILE
rm $IOS_INFORMATION_FILE

# If iOS or Android tests failed to be initiated, exit failure. Otherwise exit success
if [ $IOS_RETURN_CODE -ne 0 ] || [ $ANDROID_RETURN_CODE -ne 0 ]; then	
    exit 1
fi
