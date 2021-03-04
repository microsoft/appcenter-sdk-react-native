# Sets the target folders and the final framework product.
FMK_NAME=AppCenterReactNativeShared

# Install dir will be the final output to the framework.
# The following line create it in the root folder of the current project.
PRODUCTS_DIR=${SRCROOT}/../Products
ZIP_FOLDER=${FMK_NAME}
TEMP_DIR=${PRODUCTS_DIR}/${ZIP_FOLDER}

# Separated directories to be able to use same framework's name
INSTALL_DIR_IPHONEOS=${TEMP_DIR}/Release-iphoneos
INSTALL_DIR_IPHONESIMULATOR=${TEMP_DIR}/Release-iphonesimulator

# Temporary frameworks directories
FMK_DIR_IPHONEOS=${INSTALL_DIR_IPHONEOS}/${FMK_NAME}.framework
FMK_DIR_IPHONESIMULATOR=${INSTALL_DIR_IPHONESIMULATOR}/${FMK_NAME}.framework

# Directory of final xcframework
INSTALL_DIR_XCFRAMEWORK=${TEMP_DIR}/${FMK_NAME}.xcframework

# Working dir will be deleted after the framework creation.
DERIVED_DATA_PATH=build
WRK_DIR=${DERIVED_DATA_PATH}/Build/Products
DEVICE_DIR=${WRK_DIR}/Release-iphoneos
SIMULATOR_DIR=${WRK_DIR}/Release-iphonesimulator

# # Cleaning the oldest.
if [ -d "${TEMP_DIR}" ]
then
rm -rf "${TEMP_DIR}"
fi

create_versions_dir_for_mac_framework() {
    local framework_dir=$1
    local headers_dir=$2
    mkdir -p "${framework_dir}/Versions"

    # Create the path to the real Headers
    mkdir -p "${framework_dir}/Versions/A/Headers"

    # Create the required symlinks
    /bin/ln -sfh A "${framework_dir}/Versions/Current"

    # Copy the public headers into the framework
    /bin/cp -a ${headers_dir} "${framework_dir}/Versions/A/Headers"
}


for sdk in iphoneos iphonesimulator maccatalyst; do
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
    mkdir -p "${FMK_DIR}/Headers"
    mkdir -p "${FMK_DIR}/Modules"

    # Copy the swift import file to the temporary framework folder.
    cp -f "${SRCROOT}/${FMK_NAME}/Support/module.modulemap" "${FMK_DIR}/Modules/"

    # Copies the headers files to the temporary framework folder.
    cp -R "${WRK_DIR}/Release-${sdk}/include/${FMK_NAME}/" "${FMK_DIR}/Headers/"

    if [ ${sdk} == "maccatalyst" ]; then
        create_versions_dir_for_mac_framework ${FMK_DIR} "${FMK_DIR}/Headers/"
    fi

    # Copies the static library files to the temporary framework folder.
    SDK_DIR=${WRK_DIR}/Release-${sdk}
    cp -R "${SDK_DIR}/lib${FMK_NAME}.a" "${FMK_DIR}/${FMK_NAME}"

    frameworks+=( -framework "${FMK_DIR}")
done

# Building fina xcframework
xcodebuild -create-xcframework "${frameworks[@]}" -output "${INSTALL_DIR_XCFRAMEWORK}"

# Copies the license file to the products directory (required for cocoapods)
cp -f "../LICENSE" "${TEMP_DIR}"

for sdk in iphoneos iphonesimulator maccatalyst; do
    # Remove temporary install folders
    rm -r ${TEMP_DIR}/Release-${sdk}
done

# Remove build folder
rm -r "${DERIVED_DATA_PATH}"
