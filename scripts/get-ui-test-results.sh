#!/bin/bash

USERNAME="$MOBILE_CENTER_USERNAME" # 'MOBILE_CENTER_USERNAME' environment variable must be set
ANDROID_APP_NAME="mobilecenter-xamarin-testing-app-android"
IOS_APP_NAME="mobilecenter-xamarin-testing-app-ios"
ANDROID_APP="$USERNAME/$ANDROID_APP_NAME"
IOS_APP="$USERNAME/$IOS_APP_NAME"

# Define results constants
ANDROID_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$ANDROID_APP_NAME/test/runs/"
IOS_PORTAL_URL="https://mobile.azure.com/users/$USERNAME/apps/$IOS_APP_NAME/test/runs/"

# Log in to mobile center
./mobile-center-login.sh
if [ $? -ne 0 ]; then
	exit 1
fi

if ! [ -z ${IN_BITRISE+x} ]; then # Then we are in bitrise environment
	echo "Bitrise environment detected. Retrieving test run IDs from Azure Storage..."

	azure storage blob download -q $AZURE_STORAGE_CONTAINER $IOS_TEST_RUN_ID_FILE
	if [ $? -ne 0 ]; then
		echo "Error downloading iOS test run ID."
		exit 1
	fi
	azure storage blob download -q $AZURE_STORAGE_CONTAINER $ANDROID_TEST_RUN_ID_FILE
	if [ $? -ne 0 ]; then
		echo "Error downloading Android test run ID."
		exit 1
	fi

	IOS_TEST_RUN_ID=$(cat "$IOS_TEST_RUN_ID_FILE")
	$(echo "$IOS_TEST_RUN_ID") > $IOS_TEST_RUN_ID_FILE
	$(echo "$ANDROID_TEST_RUN_ID") > $ANDROID_TEST_RUN_ID_FILE

	echo "Test run IDs successfully retrieved."
else # Not in bitrise environment
	ANDROID_TEST_RUN_ID="$1"
	IOS_TEST_RUN_ID="$2"
fi

# Define text attributes
RED=$(tput setaf 1)
GREEN=$(tput setaf 2)
BOLD=$(tput bold)
UNATTRIBUTED=$(tput sgr0)

#test_status {TEST_RUN_ID} {APP_NAME}
#return codes: 0 == in progress, 1 == passed, 2 == failed
test_status() {
	TEST_STATUS_DONE_TEXT="Current test status: Done"
	RESULTS_FILE="results_file.txt"
	RESULT=0
	mobile-center test status --test-run-id "$1" --app "$2" > $RESULTS_FILE
	RETURN_CODE=$?
	if grep -q "$TEST_STATUS_DONE_TEXT" "$RESULTS_FILE"; then
		RESULT=1
	fi
	rm $RESULTS_FILE
	if [ $RESULT -eq 0 ]; then # still running
		echo 0
	fi
	if [ $RESULT -eq 1 ]; then
		if [ $RETURN_CODE -eq 0 ]; then # passed
			echo 1
		fi
		if [ $RETURN_CODE -ne 0 ]; then # failed
			echo 2
		fi
	fi
}

IOS_RESULT=0
ANDROID_RESULT=0
while [ $IOS_RESULT -eq 0 ] || [ $ANDROID_RESULT -eq 0 ]; do
	echo "Checking results..."
	if [ $IOS_RESULT -eq 0 ]; then
		IOS_RESULT=$(test_status "$IOS_TEST_RUN_ID" "$IOS_APP")
		if [ $IOS_RESULT -ne 0 ]; then
			echo "iOS test is done."
		fi
	fi
	if [ $ANDROID_RESULT -eq 0 ]; then
		ANDROID_RESULT=$(test_status "$ANDROID_TEST_RUN_ID" "$ANDROID_APP")
		if [ $ANDROID_RESULT -ne 0 ]; then
			echo "Android test is done."
		fi
	fi
	if [ $IOS_RESULT -eq 0 ] || [ $ANDROID_RESULT -eq 0 ]; then
		echo "Waiting 10 seconds..."
		sleep 10
	fi
done

# Print results
print_results () {
	if [ $2 -eq 1 ]; then
		echo "${BOLD}$1 test results: ${GREEN}passed! ${UNATTRIBUTED}"
	fi
	if [ $2 -eq 2 ]; then
		echo "${BOLD}$1 test results: ${RED}failed. ${UNATTRIBUTED}"
	fi
}

print_results "Android" $ANDROID_RESULT
print_results "iOS" $IOS_RESULT

echo "Full iOS test results at $IOS_PORTAL_URL$IOS_TEST_RUN_ID"
echo "Full Android test results at $ANDROID_PORTAL_URL$ANDROID_TEST_RUN_ID"

if [ $IOS_RESULT -eq 2 ] || [ $ANDROID_RESULT -eq 2 ]; then
	exit 1
fi

exit 0
