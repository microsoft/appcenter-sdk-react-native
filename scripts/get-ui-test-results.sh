#!/bin/bash

# Script usage:
#	Running locally:	./get-ui-test-results.sh {ANDROID_TEST_RUN_ID} {IOS_TEST_RUN_ID}
#	Running on bitrise: ./get-ui-test-results.sh

# Define constants

# Mobile Center constants
USERNAME="$MOBILE_CENTER_USERNAME" # 'MOBILE_CENTER_USERNAME' environment variable must be set
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
ANDROID_APP="$MOBILE_CENTER_TEST_APP_USERNAME/$ANDROID_APP_NAME"
IOS_APP="$MOBILE_CENTER_TEST_APP_USERNAME/$IOS_APP_NAME"
ANDROID_PORTAL_URL="https://mobile.azure.com/users/$MOBILE_CENTER_TEST_APP_USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://mobile.azure.com/users/$MOBILE_CENTER_TEST_APP_USERNAME/apps/$IOS_APP_NAME/test/runs/"

# Text attribute constants
RED=$(tput setaf 1)
GREEN=$(tput setaf 2)
BOLD=$(tput bold)
UNATTRIBUTED=$(tput sgr0)

# Platform name constants
IOS_NAME="iOS"
ANDROID_NAME="Android"

# Define functions

# This function downloads a file containing specified file (containing a test run id)
# from Azure Storage
# Usage: download_file {FILE_NAME} {PLATFORM_NAME}
download_file() {
	FILE_NAME="$1"
	PLATFORM_NAME="$2"
	azure storage blob download -q $AZURE_STORAGE_CONTAINER $FILE_NAME
	if [ $? -ne 0 ]; then
		echo "Error downloading $PLATFORM_NAME test run ID."
		exit 1
	fi	
}

# This function prints the test results for a given platform and return code
# Usage: print_results {PLATFORM_NAME} {RETURN_CODE}
print_results () {
	PLATFORM_NAME="$1"
	RETURN_CODE=$2
	if [ $RETURN_CODE -eq 1 ]; then
		echo "${BOLD}$PLATFORM_NAME test results: ${GREEN}passed! ${UNATTRIBUTED}"
	fi
	if [ $RETURN_CODE -eq 2 ]; then
		echo "${BOLD}$1 test results: ${RED}failed. ${UNATTRIBUTED}"
	fi
}

# This function contacts the Mobile Center backend to determine the specified test's current
# status. Echoes 0 for in progress, 1 for passed, 2 for failed.
# Usage: test_status {TEST_RUN_ID} {APP_NAME}
test_status() {
	TEST_RUN_ID="$1"
	APP_NAME="$2"
	# If and only if we see this text in the report, test is done
	TEST_STATUS_DONE_TEXT="Current test status: Done"
	RESULTS_FILE="results_file.txt"
	TEST_DONE=0
	mobile-center test status --test-run-id $TEST_RUN_ID --app "$APP_NAME" > $RESULTS_FILE
	RETURN_CODE=$?
	if grep -q "$TEST_STATUS_DONE_TEXT" "$RESULTS_FILE"; then
		TEST_DONE=1
	fi
	rm $RESULTS_FILE
	if [ $TEST_DONE -eq 0 ]; then # still running
		echo 0
	else
		if [ $RETURN_CODE -eq 0 ]; then # passed
			echo 1
		fi
		if [ $RETURN_CODE -ne 0 ]; then # failed
			echo 2
		fi
	fi
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

# If script is running in bitrise environment, then test run ids must be downloaded
if ! [ -z ${IN_BITRISE+x} ]; then # We are in bitrise environment
	echo "Bitrise environment detected. Retrieving test run IDs from Azure Storage..."
	# Download iOS test run ID from Azure Storage
	download_file $IOS_TEST_RUN_ID_FILE $IOS_NAME
	download_file $ANDROID_TEST_RUN_ID_FILE $ANDROID_NAME
	IOS_TEST_RUN_ID=$(cat "$IOS_TEST_RUN_ID_FILE")
	ANDROID_TEST_RUN_ID=$(cat "$ANDROID_TEST_RUN_ID_FILE")
	echo "Test run IDs successfully retrieved."
else # Not in bitrise environment
	# Make sure there are two arguments
	if [ -z ${2+x} ]; then
		echo "Error - usage: ./get-ui-test-results.sh {ANDROID_TEST_RUN_ID} {IOS_TEST_RUN_ID}"
		exit 1
	fi
	ANDROID_TEST_RUN_ID="$1"
	IOS_TEST_RUN_ID="$2"
fi

# Wait for results to become available, checking every ten seconds
IOS_RESULT=0
ANDROID_RESULT=0
while [ $IOS_RESULT -eq 0 ] || [ $ANDROID_RESULT -eq 0 ]; do
	echo "Checking results..."

	# Get iOS test status
	if [ $IOS_RESULT -eq 0 ]; then #iOS is still running
		IOS_RESULT=$(test_status "$IOS_TEST_RUN_ID" "$IOS_APP")
		if [ $IOS_RESULT -ne 0 ]; then
			echo "$IOS_NAME test is done."
		fi
	fi

	# Get android test status
	if [ $ANDROID_RESULT -eq 0 ]; then #Android is still running
		ANDROID_RESULT=$(test_status "$ANDROID_TEST_RUN_ID" "$ANDROID_APP")
		if [ $ANDROID_RESULT -ne 0 ]; then
			echo "$ANDROID_NAME test is done."
		fi
	fi

	# If tests are not done, wait ten seconds before continuing
	if [ $IOS_RESULT -eq 0 ] || [ $ANDROID_RESULT -eq 0 ]; then
		echo "Waiting 10 seconds..."
		sleep 10
	fi
done

# Print results
print_results $ANDROID_NAME $ANDROID_RESULT
print_results $IOS_NAME $IOS_RESULT
echo "Full $IOS_NAME test results at $IOS_PORTAL_URL$IOS_TEST_RUN_ID"
echo "Full $ANDROID_NAME test results at $ANDROID_PORTAL_URL$ANDROID_TEST_RUN_ID"

# If either test was unsuccsessful, exit failure
if [ $IOS_RESULT -eq 2 ] || [ $ANDROID_RESULT -eq 2 ]; then
	exit 1
fi
exit 0
