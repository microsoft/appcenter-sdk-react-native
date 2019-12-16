#!/bin/bash

# Use this simulator by default unless variable defined.
: ${IOS_DEVICE:="iPhone 11"}

# Start device
echo "Starting device..."
xcrun simctl boot "${IOS_DEVICE}"

# Install app
echo "Installing test app on device..."
xcrun simctl install "${IOS_DEVICE}" Tests/Contoso.Test.Functional.iOS/bin/iPhoneSimulator/Release/Contoso.Test.Functional.iOS.app

# We might need to run tests multiple times.
while true
do

    # Listen to tests
    echo "Start listening test results on socket."
    nc -l 127.0.0.1 16384 > results.xml &
    RESULTS=$!

    # Run tests
    echo "Run test app..."
    xcrun simctl launch "${IOS_DEVICE}" com.contoso.test.functional

    # Wait results
    echo "Waiting test results..."
    wait $RESULTS

    # Check if we ran a test for real
    echo "Checking test results."
    cat results.xml
    if [ "`xmllint --xpath "//*[local-name()='Counters'][@total = 0]" results.xml 2> /dev/null`" != "" ];
    then
        echo -e "\nNo test ran, retrying...".
        xcrun simctl terminate "${IOS_DEVICE}" com.contoso.test.functional
    else
        break
    fi
done

# And stop device
xcrun simctl shutdown "${IOS_DEVICE}"

# Exit with test result code (0 for success, non 0 for failure)
xmllint --xpath "//*[local-name()='Counters'][@failed = 0]" results.xml > /dev/null 2>&1
