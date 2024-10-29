#!/bin/bash

# Update AppCenter npm packages for the TestApp

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

if [ -z "$1" ] || [ "$1" == "ios" ]; then
    if [[ "$OSTYPE" == "darwin"* && "$(uname -m)" == "arm64" ]]; then
        echo "Workaround for macs on arm64"
        npm install react-native-fs --save --legacy-peer-deps
    fi

    echo "Install shared framework pods..."
    (cd ../AppCenterReactNativeShared/ios && pod install --repo-update)

    echo "Running pod install and building shared framework..."
    (cd ios && pod install --repo-update)
fi


if [ -z "$1" ] || [ "$1" == "android" ]; then
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

    # Remove versionName from build.gradle for AppCenterReactNativeShared
    # to avoid build errors like:
    # > Cannot get property 'versionName' on extra properties extension as it does not exist
    # echo "Remove versionName from build.gradle for AppCenterReactNativeShared"
    "${SED_INPLACE[@]}" "/buildConfigField .*VERSION_NAME/d" "../AppCenterReactNativeShared/android/build.gradle"

    # Remove the line "from components.release" to avoid error:
    # > Could not get unknown property 'release' for SoftwareComponentInternal set of type org.gradle.api.internal.component.DefaultSoftwareComponentContainer.
    "${SED_INPLACE[@]}" "/from components.release/d" "../AppCenterReactNativeShared/android/build.gradle"

    # Move android namespaces for the AppCenter modules from build.gradle to AndroidManifest.xml
    # to avoid build errors like:
    # > Could not find method namespace() for arguments [com.microsoft.appcenter.reactnative.appcenter] on extension 'android' of type com.android.build.gradle.LibraryExtension.
    bash ./use-android-manifest-namespaces.sh
fi