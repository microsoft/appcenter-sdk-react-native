#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Builds a framework for a platform provided in the first argument.
# Usage: ./build-framework.sh <platform> <xcodebuild arguments>

set -e

# Read arguments
CURRENT_PLATFORM=$1
BUILD_ARGUMENTS=$2

if [ -z "$CURRENT_PLATFORM" ]; then
    echo "Build platform argument should be provided"
    exit 1
fi

# Open the source directory
cd $SRCROOT
    
# Cleaning previous build
xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -quiet clean

# Building framework
if [ ${CURRENT_PLATFORM} == "maccatalyst" ]; then
    xcodebuild $BUILD_ARGUMENTS -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -destination 'platform=macOS,variant=Mac Catalyst' -derivedDataPath "$DERIVED_DATA_PATH"
else
    xcodebuild $BUILD_ARGUMENTS -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -sdk "${CURRENT_PLATFORM}" -derivedDataPath "$DERIVED_DATA_PATH"
fi

# Create and renew Release-*platform* temporary framework folder
INSTALL_DIR=${TEMP_DIR}/Release-${CURRENT_PLATFORM}
FMK_DIR=${INSTALL_DIR}/${FMK_NAME}.framework

mkdir -p "${INSTALL_DIR}"
mkdir -p "${FMK_DIR}"

BINARY_PATH="${WRK_DIR}/Release-${CURRENT_PLATFORM}/lib${FMK_NAME}.a"
HEADERS_DIR="${WRK_DIR}/Release-${CURRENT_PLATFORM}/include/${FMK_NAME}/"
MODULEMAP_PATH="${SRCROOT}/${FMK_NAME}/Support/module.modulemap"

mkdir -p "${FMK_DIR}/Headers"
mkdir -p "${FMK_DIR}/Modules"

# Copy the swift import file to the temporary framework folder.
cp -f "${MODULEMAP_PATH}" "${FMK_DIR}/Modules/"

# Copies the headers files to the temporary framework folder.
cp -R "${HEADERS_DIR}/" "${FMK_DIR}/Headers/"

# Copies the static library files to the temporary framework folder.
cp -R "${BINARY_PATH}" "${FMK_DIR}/${FMK_NAME}"
