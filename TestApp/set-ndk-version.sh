#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

check_ndk_version() {
    local ndk_path=$1
    if [ -f "$ndk_path/source.properties" ]; then
        version=$(grep "Pkg.Revision" "$ndk_path/source.properties" | cut -d' ' -f3)
        echo "$version"
    else
        echo ""
    fi
}

found_version=""

if [ -n "$ANDROID_NDK_HOME" ]; then
    echo "Checking NDK version in ANDROID_NDK_HOME..."
    found_version=$(check_ndk_version "$ANDROID_NDK_HOME")
    if [ -n "$found_version" ]; then
        echo "NDK version: $found_version (Path: $ANDROID_NDK_HOME)"
    else
        echo "NDK not found in $ANDROID_NDK_HOME"
    fi
else
    echo "ANDROID_NDK_HOME is not set."
    echo "Searching for NDK in default SDK locations..."

    if [ -n "$ANDROID_HOME" ]; then
        ndk_default_path="$ANDROID_HOME/ndk"
    else
        ndk_default_path="$HOME/Library/Android/sdk/ndk"
    fi

    if [ -d "$ndk_default_path" ]; then
        for ndk_dir in "$ndk_default_path"/*; do
            if [ -d "$ndk_dir" ]; then
                found_version=$(check_ndk_version "$ndk_dir")
                if [ -n "$found_version" ]; then
                    echo "NDK version: $found_version (Path: $ndk_dir)"
                    break
                fi
            fi
        done
    else
        echo "NDK not found in default SDK path: $ndk_default_path"
    fi
fi

if [ -z "$found_version" ]; then
    echo "No NDK version found."
fi

# PROPERTY_FILE="$ANDROID_HOME/ndk-bundle/source.properties"

# echo "Reading ndk version from source.properties file..."
# VERSION=$(cat $PROPERTY_FILE | sed 's/ //g' | grep "Pkg.Revision" | cut -d'=' -f2)
# echo $VERSION

$VERSION = $found_version
GRADLE_FILE="./android/app/build.gradle"

if [ -z $VERSION ]; then
  echo "No NDK found in the default location. Proceeding..."
else

  # Insert ndkVersion = 'x.x.x' in the android section.
  NDK_VERSION_LINE="ndkVersion = '$VERSION'"
  echo "$(sed "s/android {/android {\\`echo -e '\n\r    '`$NDK_VERSION_LINE/g" "$GRADLE_FILE")" > $GRADLE_FILE
fi