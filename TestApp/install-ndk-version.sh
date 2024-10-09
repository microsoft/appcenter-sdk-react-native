#!/bin/bash

if [ -z "$1" ]; then
    echo "There is no NDK_MAJOR_VERSION variable provided"
    return 1
fi

if [ -z "$ANDROID_HOME" ]; then
    echo "ANDROID_HOME is not defined"
    return 1
fi

NDK_MAJOR_VERSION="$1"
echo "NDK_MAJOR_VERSION is $NDK_MAJOR_VERSION"

echo "Download Android NDK version $NDK_MAJOR_VERSION"
curl -o android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip https://dl.google.com/android/repository/android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip
unzip android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip -d android-ndk-r$NDK_MAJOR_VERSION

echo "Copy NDK to $ANDROID_HOME/ndk-bundle"
rm -rf $ANDROID_HOME/ndk-bundle
cp -r ./android-ndk-r$NDK_MAJOR_VERSION/android-ndk-r$NDK_MAJOR_VERSION $ANDROID_HOME/ndk-bundle

rm -rf android-ndk-r$NDK_MAJOR_VERSION android-ndk-r$NDK_MAJOR_VERSION-darwin-x86_64.zip

echo "NDK $NDK_MAJOR_VERSION is installed to $ANDROID_HOME/ndk-bundle"