#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Update AppCenter npm packages for the TestApp

if [ -z "$1" ]; then
    echo "Running for both Android and iOS"
    is_android=true
    is_ios=true
else 
    case "$1" in
        android)
            echo "Running for Android"
            is_android=true
            is_ios=false
            ;;
        ios)
            echo "Running for iOS"
            is_android=false
            is_ios=true
            ;;
        *)
            echo "Invalid argument: $1"
            exit 1
            ;;
    esac
fi

set -e

echo 'Removing existing appcenter* packages...'
rm -rf node_modules/appcenter*

echo "Packing appcenter* packages..."
npm pack ../appcenter
npm pack ../appcenter-analytics
npm pack ../appcenter-crashes
npm pack ../appcenter-link-scripts

echo "Installing appcenter* packages..."
npm install appcenter*.tgz

echo "Cleanup appcenter*.tgz"
rm appcenter*.tgz

echo "Installing other packages..."
npm install

if [ "$is_ios" == "true" ]; then
    echo "Workaround for macs on arm64"
    npm install react-native-fs --save --legacy-peer-deps

    echo "Applying patch to react-native package that fixes build on Xcode 12.5..."
    npx patch-package

    echo "Updating CocoaPods repos..."
    pod repo update

    echo "Install shared framework pods..."
    (cd ../AppCenterReactNativeShared/ios && pod install)

    echo "Running pod install and building shared framework..."
    (cd ios && pod install --repo-update)
fi


if [ "$is_android" == "true" ]; then
    echo "Running jetify to resolve AndroidX compatibility issues..."
    npx jetify

    OS_TYPE=$(uname)
    if [[ "$OS_TYPE" == "Darwin" ]]; then
        # macOS
        SED_INPLACE=("sed" "-i" "")
    else
        # Linux and others
        SED_INPLACE=("sed" "-i")
    fi

    # Remove versionName and versionCode from build.gradle for AppCenterReactNativeShared
    # to avoid build errors like:
    # > Could not find method namespace() for arguments [com.microsoft.appcenter.reactnative.appcenter] on extension 'android' of type com.android.build.gradle.LibraryExtension.
    echo "Remove versionName and versionCode from build.gradle for AppCenterReactNativeShared"
    "${SED_INPLACE[@]}" "/buildConfigField .*VERSION_NAME/d" "../AppCenterReactNativeShared/android/build.gradle"
    "${SED_INPLACE[@]}" "/from components.release/d" "../AppCenterReactNativeShared/android/build.gradle"

    # Move android namespaces for the AppCenter modules from build.gradle to AndroidManifest.xml
    bash ./use-android-manifest-namespaces.sh
fi