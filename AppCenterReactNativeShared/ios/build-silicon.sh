#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Builds arm64 slices under Xcode 12 for provided IOS_PLATFORMS argument.
# Usage: ./build-silicon.sh
# Note: Requires SRCROOT, IOS_PLATFORMS, FMK_NAME.

set -e

PRODUCTS_DIR=${SRCROOT}/../Products
ZIP_FOLDER=${FMK_NAME}
export TEMP_DIR=${PRODUCTS_DIR}/${ZIP_FOLDER}

# Working dir will be deleted after the framework creation.
export DERIVED_DATA_PATH=build
export WRK_DIR=${DERIVED_DATA_PATH}/Build/Products

# Cleaning the old build
if [ -d "${TEMP_DIR}" ]; then
rm -rf "${TEMP_DIR}"
fi

for sdk in $IOS_PLATFORMS; do
    $SRCROOT/build-framework.sh $sdk "ONLY_ACTIVE_ARCH=NO ARCHS=arm64"
done

# Remove build folder
rm -r "${SRCROOT}/${DERIVED_DATA_PATH}"
