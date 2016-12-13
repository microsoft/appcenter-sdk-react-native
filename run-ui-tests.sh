#!/bin/bash

# Define constants
BUILD_SCRIPT=build.sh
BUILD_TARGET=Tests
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
NPM_INSTALL=$TOOLS_DIR/npm_install.sh
NPM_URL=https://www.npmjs.org/install.sh
TEST_APK=$SCRIPT_DIR/Tests/Droid/bin/Release/com.contoso.contoso_forms_test.apk
TEST_IPA=$SCRIPT_DIR/Tests/iOS/bin/iPhone/Release/Contoso.Forms.Test.iOS.ipa
UITEST_ASSEMBLY_DIR=$SCRIPT_DIR/Tests/UITests/bin/Release
LOCALE="en-US"
USERNAME="$1"
PASSWORD="$2"
IOS_DEVICES=8551ba4e
ANDROID_DEVICES=f0b8289c
ANDROID_APP="$USERNAME/mobilecenter-xamarin-testing-app-android"
IOS_APP="$USERNAME/mobilecenter-xamarin-testing-app-ios"
TEST_SERIES="master"

# Make sure that the tools directory exists
if [ ! -d $TOOLS_DIR ]; then
  mkdir $TOOLS_DIR
fi

# Download and install NPM if it is not already
npm -v &>/dev/null
if [ $? -ne 0 ]; then
	# Download npm
	echo "Downloading npm..."
    brew install npm
	if [ $? -ne 0 ]; then
    	echo "An error occured while downloading npm."
    	exit 1
	fi

	# Install npm
	echo "Installing npm..."
	sh $NPM_INSTALL >/dev/null
	if [ $? -ne 0 ]; then
    	echo "An error occured while installing npm."
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

#2 Log in to Mobile Center
echo "Logging in to mobile center as $USERNAME..."
mobile-center login -u "$1" -p "$PASSWORD"
if [ $? -ne 0 ]; then
    echo "An error occured while logging into Mobile Center."
    exit 1
fi

#4 Build tests
echo "Building applications and UITests..."
sh $BUILD_SCRIPT -t $BUILD_TARGET
if [ $? -ne 0 ]; then
    echo "An error occured while building tests."
    exit 1
fi

# Upload tests
echo "Uploading Android tests..."
mobile-center test run uitest --app $ANDROID_APP\
 --devices $ANDROID_DEVICES --app-path $TEST_APK\
  --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_ASSEMBLY_DIR
echo "Uploading iOS tests..."
mobile-center test run uitest --app $IOS_APP\
   --devices $IOS_DEVICES --app-path $TEST_IPA\
   --test-series $TEST_SERIES --locale $LOCALE\
   --build-dir $UITEST_ASSEMBLY_DIR
