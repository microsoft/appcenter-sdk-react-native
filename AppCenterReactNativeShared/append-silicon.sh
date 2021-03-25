#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Builds arm64 slices under Xcode 12 and appends them to xcramework built under Xcode 11.
# Usage: export SRCROOT=`pwd` && ./append-silicon.sh
# Note: This script should be called after build-xcframework succeed under Xcode 11.

set -e

export FMK_NAME=AppCenterReactNativeShared
PRODUCT_DIRECTORY=`pwd`/Products/AppCenterReactNativeShared
XCODE_11_PRODUCT_DIRECTORY=`pwd`/Products/AppCenterReactNativeShared-xcode11

if [ ! -d "${PRODUCT_DIRECTORY}" ] || [ -z "$(ls -A $PRODUCT_DIRECTORY)" ]; then
    echo "Directory $PRODUCT_DIRECTORY does not exist or is empty"
    exit 1
fi
if [ -d "${XCODE_11_PRODUCT_DIRECTORY}" ]; then
    echo "Clean $XCODE_11_PRODUCT_DIRECTORY directory"
    rm -rf "${XCODE_11_PRODUCT_DIRECTORY}"
fi

echo "Copy resources built with Xcode 11 to another folder"
cp -R $PRODUCT_DIRECTORY $XCODE_11_PRODUCT_DIRECTORY

echo "Clean directory for new Xcode build products"
rm -rf "${PRODUCT_DIRECTORY}"

echo "Build xcframeworks with new Xcode"
export SRCROOT=`pwd`/ios
export IOS_PLATFORMS="maccatalyst iphonesimulator"

./ios/build-silicon.sh

append_to_framework() {
  if [ ! -e "$1" ]; then
    return 1
  fi
  local binary="$1/$FMK_NAME"
  [[ ! -e "$binary" ]] && binary="$1/$FMK_NAME"
  lipo "$binary" "$2/$FMK_NAME" -create -output "$binary"
  lipo -info "$binary"
}

append_to_xcframework() {
  append_to_framework \
    "$XCODE_11_PRODUCT_DIRECTORY/$FMK_NAME.xcframework/$1/$FMK_NAME.framework" \
    "$PRODUCT_DIRECTORY/Release-$2/$FMK_NAME.framework"
}

echo "Append arm64 slices to binaries built under Xcode 11"
append_to_xcframework ios-i386_x86_64-simulator iphonesimulator
append_to_xcframework ios-x86_64-maccatalyst maccatalyst

for framework in $XCODE_11_PRODUCT_DIRECTORY/$FMK_NAME.xcframework/*/$FMK_NAME.framework; do
  xcframeworks+=( -framework "$framework")
  echo $framework
done

echo "Clean product directory for the full xcframework"
rm -rf "${PRODUCT_DIRECTORY}"

echo "Repackage xcframework with additional architectures"
xcodebuild -create-xcframework "${xcframeworks[@]}" -output "$PRODUCT_DIRECTORY/$FMK_NAME.xcframework"

echo "Copy the license file to the products directory (required for cocoapods)"
cp -f "$XCODE_11_PRODUCT_DIRECTORY/LICENSE" "$PRODUCT_DIRECTORY"

ls "$PRODUCT_DIRECTORY/$FMK_NAME.xcframework"
