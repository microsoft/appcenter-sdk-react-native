#!/bin/bash
set -e

echo 'Bundling AppcenterReactNativeShared...'
# Java 11 version is required for running gradle tasks
java_version=$(java -version 2>&1 | awk -F[\".] 'NR==1{print $2}')
if [[ "$java_version" != "11" ]]; then
  echo "Java 11 is required. Current version: $java_version"
  exit 1
fi
cd ../AppCenterReactNativeShared/android
./gradlew publishToMavenLocal  
cd ../../TestApp

echo 'Removing existing appcenter* packages...'
rm -rf node_modules/appcenter*

echo "Packing appcenter* packages..."
npm pack ../appcenter
npm pack ../appcenter-analytics
npm pack ../appcenter-crashes
npm pack ../appcenter-link-scripts

echo "Installing appcenter* packages..."
npm install appcenter*.tgz

echo "workaround for macs on arm64"
npm install react-native-fs --save --legacy-peer-deps

echo "Cleanup appcenter*.tgz"
rm appcenter*.tgz

echo "Installing other packages..."
npm install

echo "Updating CocoaPods repos..."
pod repo update

echo "Install shared framework pods..."
(cd ../AppCenterReactNativeShared/ios && pod install)

# for testing with not released apple and android sdks, you will need to provide the storage access key
echo "Running pod install and building shared framework..."
(cd ios && pod install --repo-update)
