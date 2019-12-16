#!/bin/bash

# We can't run emulator as a daemon
# VSTS will not execute next step until emulator killed
# So we need to run tests in same step...
export DYLD_LIBRARY_PATH="$ANDROID_HOME/emulator/lib64/qt/lib"
$ANDROID_HOME/emulator/emulator -avd emulator -skin 768x1280 -no-window -gpu off &

# Ensure Android Emulator has booted successfully before continuing
EMU_BOOTED='unknown'
while [[ ${EMU_BOOTED} != *"stopped"* ]]; do
    echo "Waiting emulator to start..."
    sleep 5
    EMU_BOOTED=`adb shell getprop init.svc.bootanim || echo unknown`
done
duration=$(( SECONDS - start ))
echo "Android Emulator started after $duration seconds."

# Install app
echo "Installing Android test app on device..."
adb install Tests/Contoso.Test.Functional.Droid/bin/Release/com.contoso.test.functional-Signed.apk

# Listen to tests
echo "Start listening test results on socket."
nc -l 127.0.0.1 16384 > results.xml &
RESULTS=$!

# Run tests
echo "Run test app..."
adb shell monkey -p com.contoso.test.functional -c android.intent.category.LAUNCHER 1

# While waiting, print AppCenter logs if any
adb logcat | grep AppCenter &
LOGCAT_PID=$!

# Wait results
echo "Waiting test results..."
wait $RESULTS
kill $LOGCAT_PID

# Check if a test failed, also check at least 1 ran as sometimes we get a 0 report...
echo "Checking test results."
cat results.xml
xmllint --xpath "//*[local-name()='Counters'][@passed > 0 and @failed = 0]" results.xml > /dev/null 2>&1
TEST_RESULT=$?

# And kill emulator
adb emu kill

# Exit with test result code (0 for success, non 0 for failure)
exit $TEST_RESULT
