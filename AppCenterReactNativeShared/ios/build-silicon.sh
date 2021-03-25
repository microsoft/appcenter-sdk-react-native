#!/bin/bash
# Build arm64 slices on CI separately as Xcode 11.3 doesn't build them.

PRODUCTS_DIR=${SRCROOT}/../Products
ZIP_FOLDER=${FMK_NAME}
export TEMP_DIR=${PRODUCTS_DIR}/${ZIP_FOLDER}

# Working dir will be deleted after the framework creation.
export DERIVED_DATA_PATH=build
export WRK_DIR=${DERIVED_DATA_PATH}/Build/Products

# # Cleaning the oldest.
if [ -d "${TEMP_DIR}" ]; then
rm -rf "${TEMP_DIR}"
fi

for sdk in $IOS_PLATFORMS; do

    # Build framework with arguments: $1=current-platform, $2=xcodebuild arguments
    $SRCROOT/build-framework.sh $sdk "ONLY_ACTIVE_ARCH=NO ARCHS=arm64"
done

# Remove build folder
rm -r "${SRCROOT}/${DERIVED_DATA_PATH}"
