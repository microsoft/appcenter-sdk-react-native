#!/bin/bash
./update-npm-packages.sh
echo 'Unlinking...'
react-native unlink mobile-center
react-native unlink mobile-center-crashes
react-native unlink mobile-center-analytics
react-native unlink mobile-center-push
echo 'Updating cocoapods repo'
pod repo update
echo 'Removing old Pods...'
rm -rf ios/Pods
rm -rf ios/Podfile.lock
echo 'Copying RNMobileCenter pod manually'
mkdir -p ios/Pods/RNMobileCenterShared
cp -R ../RNMobileCenterShared/Products/RNMobileCenterShared ios/Pods/RNMobileCenterShared
echo 'Linking...'
react-native link
