#!/bin/bash

if [ -z "$1" ]; then
    echo "There is no NDK_VERSION variable provided"
    return 1
fi

NDK_VERSION="$1"
echo "NDK_VERSION is $NDK_VERSION"

echo "Download Android NDK version $NDK_VERSION"
curl -o android-ndk-r$NDK_VERSION-darwin-x86_64.zip https://dl.google.com/android/repository/android-ndk-r$NDK_VERSION-darwin-x86_64.zip
unzip android-ndk-r$NDK_VERSION-darwin-x86_64.zip -d $HOME
export ANDROID_NDK_HOME=$HOME/android-ndk-r$NDK_VERSION
echo "ANDROID_NDK_HOME set as $ANDROID_NDK_HOME"
echo "##vso[task.setvariable variable=ANDROID_NDK_HOME]$HOME/android-ndk-r$NDK_VERSION"