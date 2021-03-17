# Sets the target folders and the final framework product.
FMK_NAME=AppCenterReactNativeShared

# SRCROOT="."

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

create_mac_framework() {
    local framework_dir=$1
    local headers_dir=$2
    local binary_path=$3

    mkdir -p "${framework_dir}/Versions"

    # Create the path to the real Headers
    mkdir -p "${framework_dir}/Versions/A/Headers"

    # Copy static library file to Versions/A
    cp -R "${binary_path}" "${framework_dir}/Versions/A/${FMK_NAME}"

    # Create the required symlinks
    /bin/ln -sfh A "${framework_dir}/Versions/Current"
    /bin/ln -sfh "A/Headers" "${framework_dir}/Headers"
    /bin/ln -sfh "A/${FMK_NAME}" "${framework_dir}/${FMK_NAME}"

    # Copy the public headers into the framework
    /bin/cp -a ${headers_dir} "${framework_dir}/Versions/A/Headers"
}

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

    # # Copy the swift import file to the temporary framework folder.
    # cp -f "${SRCROOT}/${FMK_NAME}/Support/module.modulemap" "${FMK_DIR}/Modules/"

    # # Copies the headers files to the temporary framework folder.
    # cp -R "${WRK_DIR}/Release-${sdk}/include/${FMK_NAME}/" "${FMK_DIR}/Headers/"

    binary_path="${WRK_DIR}/Release-${sdk}/lib${FMK_NAME}.a"
    headers_dir="${WRK_DIR}/Release-${sdk}/include/${FMK_NAME}/"
    modulemap_path="${SRCROOT}/${FMK_NAME}/Support/module.modulemap"

    if [ ${sdk} == "maccatalyst" ]; then
        mkdir -p "${FMK_DIR}/Versions"
        mkdir -p "${FMK_DIR}/Versions/A/Headers"
        mkdir "${FMK_DIR}/Versions/A/Modules"

        # Copy static library file to Versions/A
        cp -R "${binary_path}" "${FMK_DIR}/Versions/A/${FMK_NAME}"
        # cp -R "${binary_path}" "${FMK_DIR}/${FMK_NAME}"

        # Create the required symlinks
        ln -sfh A "${FMK_DIR}/Versions/Current"
        ln -sfh "Versions/A/Headers" "${FMK_DIR}/Headers"
        ln -sfh "Versions/A/Modules" "${FMK_DIR}/Modules"
        ln -sfh "Versions/A/${FMK_NAME}" "${FMK_DIR}/${FMK_NAME}"

        # Copy the public headers into the framework
        cp -f "${modulemap_path}" "${FMK_DIR}/Versions/A/Modules/"
        cp -R "${headers_dir}/" "${FMK_DIR}/Versions/A/Headers/"

        # create_mac_framework ${FMK_DIR} "${WRK_DIR}/Release-${sdk}/include/${FMK_NAME}/" "${SDK_DIR}/lib${FMK_NAME}.a"
    else
        mkdir -p "${FMK_DIR}/Headers"
        mkdir -p "${FMK_DIR}/Modules"

        # Copy the swift import file to the temporary framework folder.
        cp -f "${modulemap_path}" "${FMK_DIR}/Modules/"

        # Copies the headers files to the temporary framework folder.
        cp -R "${headers_dir}/" "${FMK_DIR}/Headers/"

        # Copies the static library files to the temporary framework folder.
        # SDK_DIR=${WRK_DIR}/Release-${sdk}
        cp -R "${binary_path}" "${FMK_DIR}/${FMK_NAME}"
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
