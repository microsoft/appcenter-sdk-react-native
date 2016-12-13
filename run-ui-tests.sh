#!/bin/bash

# Define directory and file locations
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TEST_APK=$SCRIPT_DIR/Tests/Droid/bin/Release/com.contoso.contoso_forms_test.apk
TEST_IPA=$SCRIPT_DIR/Tests/iOS/bin/iPhone/Release/Contoso.Forms.Test.iOS.ipa
UITEST_BUILD_DIR=$SCRIPT_DIR/Tests/UITests/bin/Release

# Define build constants
BUILD_SCRIPT=build.sh
BUILD_TARGET=Tests

# Define test parameters
LOCALE="en-US"
USERNAME="$MOBILE_CENTER_USERNAME" # 'MOBILE_CENTER_USERNAME' environment variable must be set
PASSWORD="$MOBILE_CENTER_PASSWORD" # 'MOBILE_CENTER_PASSWORD' environment variable must be set
IOS_DEVICES=8551ba4e # just one device. For a suite of 40, use 118f9d2f
ANDROID_DEVICES=f0b8289c # just one device. For a suite of 40, use f47808f1
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
ANDROID_APP="$USERNAME/$ANDROID_APP_NAME"
IOS_APP="$USERNAME/$IOS_APP_NAME"
TEST_SERIES="master"

# Define results constants
EXIT_CODE=0
ANDROID_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$IOS_APP_NAME/test/runs/"
ANDROID_RESULTS_FILE="android_results_$(date).txt"
IOS_RESULTS_FILE="android_results_$(date).txt"

# Define text attributes
RED=$(tput setaf 1)
GREEN=$(tput setaf 2)
BOLD=$(tput bold)
UNATTRIBUTED=$(tput sgr0)

# Download and install NPM if it is not already installed
npm -v &>/dev/null
if [ $? -ne 0 ]; then
	# Install npm
	echo "Installing npm..."
    brew install npm
	if [ $? -ne 0 ]; then
    	echo "An error occured while downloading npm."
    	exit 1
	fi 
fi

# Is Mobile Center CLI installed?
npm list -g mobile-center-cli >/dev/null
if [ $? -ne 0 ]; then
	# Install Mobile Center CLI
	echo "Installing Mobile Center CLI..."
	npm install -g mobile-center-cli
	if [ $? -ne 0 ]; then
    	echo "An error occured while installing Mobile Center CLI."
    	exit 1
	fi
fi

# Log in to Mobile Center
echo "Logging in to mobile center as $USERNAME..."
mobile-center login -u "$USERNAME" -p "$PASSWORD"
if [ $? -ne 0 ]; then
    echo "An error occured while logging into Mobile Center."
    exit 1
fi

# Build tests
echo "Building applications and UITests..."
sh $BUILD_SCRIPT -t $BUILD_TARGET
if [ $? -ne 0 ]; then
    echo "An error occured while building tests."
    exit 1
fi

# Run Android tests
echo "[$(date)] Running Android tests..."
mobile-center test run uitest --app $ANDROID_APP\
 --devices $ANDROID_DEVICES --app-path $TEST_APK\
  --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_BUILD_DIR > $ANDROID_RESULTS_FILE
ANDROID_RETURN_CODE=$?
echo "[$(date)] Android tests completed."
cat $ANDROID_RESULTS_FILE
ANDROID_TEST_RUN_ID=$(
while read -r line
do
	if [ $(expr "$line" : "Test run id: ") -ne 0 ]; then
		echo $(echo $line | cut -d'"' -f 2)
		break
	fi
done < $ANDROID_RESULTS_FILE)
rm $ANDROID_RESULTS_FILE

# Run iOS tests
echo "Running iOS tests..."
mobile-center test run uitest --app $IOS_APP\
   --devices $IOS_DEVICES --app-path $TEST_IPA\
   --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_BUILD_DIR > $IOS_RESULTS_FILE
IOS_RETURN_CODE=$?
echo "[$(date)] iOS tests completed."
cat $IOS_RESULTS_FILE
IOS_TEST_RUN_ID=$(
while read -r line
do
   	if [ $(expr "$line" : "Test run id: ") -ne 0 ]; then
		echo $(echo $line | cut -d'"' -f 2)
		break
	fi
done < $IOS_RESULTS_FILE)
rm $IOS_RESULTS_FILE

# Print results
if [ $ANDROID_RETURN_CODE -eq 0 ]; then
	echo "${BOLD}Android test results: ${GREEN}passed! ${UNATTRIBUTED}"
fi
if [ $ANDROID_RETURN_CODE -ne 0 ]; then
	EXIT_CODE=$ANDROID_RETURN_CODE
	echo "${BOLD}Android test results: ${RED}failed.${UNATTRIBUTED}${BOLD} For more information, visit $ANDROID_PORTAL_URL$ANDROID_TEST_RUN_ID.${UNATTRIBUTED}"
fi
if [ $IOS_RETURN_CODE -eq 0 ]; then
	echo "${BOLD}iOS test results: ${GREEN}passed! ${UNATTRIBUTED}"
fi
if [ $IOS_RETURN_CODE -ne 0 ]; then
	EXIT_CODE=$IOS_RETURN_CODE
	echo "${BOLD}iOS test results: ${RED}failed.${UNATTRIBUTED}${BOLD} For more information, visit $IOS_PORTAL_URL$IOS_TEST_RUN_ID.${UNATTRIBUTED}"
fi

exit $EXIT_CODE
