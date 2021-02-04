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

# Cleaning previous build
xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" clean

# Building Release-iphoneos temporary framework.
xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -sdk iphoneos -derivedDataPath "$DERIVED_DATA_PATH"

# Creates and renews Release-iphoneos temporary framework folder.
mkdir -p "${INSTALL_DIR_IPHONEOS}"
mkdir -p "${FMK_DIR_IPHONEOS}"
mkdir -p "${FMK_DIR_IPHONEOS}/Headers"
mkdir -p "${FMK_DIR_IPHONEOS}/Modules"

# Copy the swift import file to the temporary framework folder.
cp -f "${SRCROOT}/${FMK_NAME}/Support/module.modulemap" "${FMK_DIR_IPHONEOS}/Modules/"

# Copies the headers files to the temporary framework folder.
cp -R "${WRK_DIR}/Release-iphoneos/include/${FMK_NAME}/" "${FMK_DIR_IPHONEOS}/Headers/"

# Copies the static library files to the temporary framework folder.
cp -R "${DEVICE_DIR}/lib${FMK_NAME}.a" "${FMK_DIR_IPHONEOS}/${FMK_NAME}"

# Building Release-iphonesimulator temporary framework.
xcodebuild -workspace "${FMK_NAME}.xcworkspace" -configuration "Release" -scheme "${FMK_NAME}" -sdk iphonesimulator -derivedDataPath "$DERIVED_DATA_PATH"

# Creates and renews Release-iphonesimulator temporary framework folder.
mkdir -p "${INSTALL_DIR_IPHONESIMULATOR}"
mkdir -p "${FMK_DIR_IPHONESIMULATOR}"
mkdir -p "${FMK_DIR_IPHONESIMULATOR}/Headers"
mkdir -p "${FMK_DIR_IPHONESIMULATOR}/Modules"

# Copy the swift import file to the temporary framework folder.
cp -f "${SRCROOT}/${FMK_NAME}/Support/module.modulemap" "${FMK_DIR_IPHONESIMULATOR}/Modules/"

# Copies the headers files to the temporary framework folder.
cp -R "${WRK_DIR}/Release-iphonesimulator/include/${FMK_NAME}/" "${FMK_DIR_IPHONESIMULATOR}/Headers/"

# Copies the static library files to the temporary framework folder.
cp -R "${SIMULATOR_DIR}/lib${FMK_NAME}.a" "${FMK_DIR_IPHONESIMULATOR}/${FMK_NAME}"

# Building fina xcframework
xcodebuild -create-xcframework -framework "${FMK_DIR_IPHONEOS}" -framework "${FMK_DIR_IPHONESIMULATOR}" -output "${INSTALL_DIR_XCFRAMEWORK}"

# Copies the license file to the products directory (required for cocoapods)
cp -f "../LICENSE" "${TEMP_DIR}"

# Remove temporary install folders
rm -r "${INSTALL_DIR_IPHONEOS}"
rm -r "${INSTALL_DIR_IPHONESIMULATOR}"

# Remove build folder
rm -r "${DERIVED_DATA_PATH}"