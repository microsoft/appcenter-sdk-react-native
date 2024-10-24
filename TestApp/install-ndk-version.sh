#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Download and install the specified version of the Android NDK

# Check if the NDK_MAJOR_VERSION argument is provided
if [ -z "$1" ]; then
    echo "NDK version is not provided"
    return 1
fi

# Check if ANDROID_HOME is defined
if [ -z "$ANDROID_HOME" ]; then
    echo "ANDROID_HOME is not defined"
    return 1
fi

NDK_MAJOR_VERSION="$1"
ANDROID_NDK_HOME="$ANDROID_HOME/ndk-bundle"

# Download the specified version of the Android NDK
echo "Download Android NDK version $NDK_MAJOR_VERSION"
curl -o android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip https://dl.google.com/android/repository/android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip

# Unzip the downloaded NDK zip file into a directory
unzip android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip -d android-ndk-r$NDK_MAJOR_VERSION

# Remove any existing NDK installation in ANDROID_HOME/ndk-bundle and replace it with the downloaded one
echo "Copy NDK to $ANDROID_NDK_HOME"
rm -rf $ANDROID_NDK_HOME
cp -r ./android-ndk-r$NDK_MAJOR_VERSION/android-ndk-r$NDK_MAJOR_VERSION $ANDROID_NDK_HOME

# Clean up by removing the unzipped files and the downloaded zip file
rm -rf android-ndk-r$NDK_MAJOR_VERSION android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip

echo "NDK $NDK_MAJOR_VERSION is installed to $ANDROID_NDK_HOME"