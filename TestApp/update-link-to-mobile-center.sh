#!/bin/bash
set -e

./update-npm-packages.sh

echo 'Unlinking...'
react-native unlink appcenter
react-native unlink appcenter-crashes
react-native unlink appcenter-analytics
react-native unlink appcenter-push

echo 'Updating cocoapods repo'
pod repo update

echo 'Removing old Pods and Podfile.lock'
rm -rf ios/Pods
rm -rf ios/Podfile.lock

echo 'Copying AppCenterReactNativeShared pod manually'
mkdir -p ios/Pods/AppCenterReactNativeShared
cp -R ../AppCenterReactNativeShared/Products/AppCenterReactNativeShared ios/Pods/AppCenterReactNativeShared

echo 'Linking...'
react-native link
