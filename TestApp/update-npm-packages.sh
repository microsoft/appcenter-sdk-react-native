#!/bin/bash
set -e

echo 'Removing existing appcenter* packages...'
rm -rf node_modules/appcenter*

echo "Packing appcenter* packages..."
npm pack ../appcenter
npm pack ../appcenter-analytics
npm pack ../appcenter-crashes
npm pack ../appcenter-link-scripts
# workaround for macs on arm64
npm install react-native-fs --save --legacy-peer-deps

echo "Installing appcenter* packages..."
npm install appcenter*.tgz

echo "Cleanup appcenter*.tgz"
rm appcenter*.tgz

echo "Installing other packages..."
npm install

echo "Running jetify to resolve AndroidX compatibility issues..."
npx jetify

echo "Updating CocoaPods repos..."
pod repo update

echo "Install shared framework pods..."
(cd ../AppCenterReactNativeShared/ios && pod install)

echo "Running pod install and building shared framework..."
(cd ios && pod install)

echo "Copy shared framework pod..."
cp -r ../AppCenterReactNativeShared/Products/AppCenterReactNativeShared ios/Pods/AppCenterReactNativeShared

# workaround for macs on arm64 (uncomment when running on arm64 mac)
# (cd ios && pod install)
