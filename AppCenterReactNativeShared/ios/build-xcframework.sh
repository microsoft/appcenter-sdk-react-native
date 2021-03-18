# Sets the target folders and the final framework product.
FMK_NAME=AppCenterReactNativeShared

# Install dir will be the final output to the framework.
# The following line create it in the root folder of the current project.
PRODUCTS_DIR=${SRCROOT}/../Products
ZIP_FOLDER=${FMK_NAME}
TEMP_DIR=${PRODUCTS_DIR}/${ZIP_FOLDER}

# Directory of final xcframework
INSTALL_DIR_XCFRAMEWORK=${TEMP_DIR}/${FMK_NAME}.xcframework

# Working dir will be deleted after the framework creation.
DERIVED_DATA_PATH=build
WRK_DIR=${DERIVED_DATA_PATH}/Build/Products

# # Cleaning the oldest.
if [ -d "${TEMP_DIR}" ]; then
rm -rf "${TEMP_DIR}"
fi

for sdk in maccatalyst iphoneos iphonesimulator; do
    # Cleaning previous build
    xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" clean

    # Building framework.
    if [ ${sdk} == "maccatalyst" ]; then
        xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -destination 'platform=macOS,variant=Mac Catalyst' -derivedDataPath "$DERIVED_DATA_PATH"
    else
        xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -sdk "${sdk}" -derivedDataPath "$DERIVED_DATA_PATH"
    fi

    # Creates and renews Release-iphoneos temporary framework folder.
    INSTALL_DIR=${TEMP_DIR}/Release-${sdk}
    FMK_DIR=${INSTALL_DIR}/${FMK_NAME}.framework

    mkdir -p "${INSTALL_DIR}"
    mkdir -p "${FMK_DIR}"

    BINARY_PATH="${WRK_DIR}/Release-${sdk}/lib${FMK_NAME}.a"
    HEADERS_DIR="${WRK_DIR}/Release-${sdk}/include/${FMK_NAME}/"
    MODULEMAP_PATH="${SRCROOT}/${FMK_NAME}/Support/module.modulemap"

    if [ ${sdk} == "maccatalyst" ]; then
        mkdir -p "${FMK_DIR}/Versions"
        mkdir -p "${FMK_DIR}/Versions/A/Headers"
        mkdir "${FMK_DIR}/Versions/A/Modules"

        # Copies the static library files to the temporary framework folder.
        cp -R "${BINARY_PATH}" "${FMK_DIR}/Versions/A/${FMK_NAME}"

        # Create the required symlinks
        ln -sfh A "${FMK_DIR}/Versions/Current"
        ln -sfh "Versions/A/Headers" "${FMK_DIR}/Headers"
        ln -sfh "Versions/A/Modules" "${FMK_DIR}/Modules"
        ln -sfh "Versions/A/${FMK_NAME}" "${FMK_DIR}/${FMK_NAME}"

        # Copy the public headers into the framework
        cp -f "${MODULEMAP_PATH}" "${FMK_DIR}/Versions/A/Modules/"
        cp -R "${HEADERS_DIR}/" "${FMK_DIR}/Versions/A/Headers/"
    else
        mkdir -p "${FMK_DIR}/Headers"
        mkdir -p "${FMK_DIR}/Modules"

        # Copy the swift import file to the temporary framework folder.
        cp -f "${MODULEMAP_PATH}" "${FMK_DIR}/Modules/"

        # Copies the headers files to the temporary framework folder.
        cp -R "${HEADERS_DIR}/" "${FMK_DIR}/Headers/"

        # Copies the static library files to the temporary framework folder.
        cp -R "${BINARY_PATH}" "${FMK_DIR}/${FMK_NAME}"
    fi

    frameworks+=( -framework "${FMK_DIR}")
done

# Building final xcframework
xcodebuild -create-xcframework "${frameworks[@]}" -output "${INSTALL_DIR_XCFRAMEWORK}"

# Copies the license file to the products directory (required for cocoapods)
cp -f "../LICENSE" "${TEMP_DIR}"

for sdk in iphoneos iphonesimulator maccatalyst; do
    # Remove temporary install folders
    rm -r ${TEMP_DIR}/Release-${sdk}
done

# Remove build folder
rm -r "${DERIVED_DATA_PATH}"
