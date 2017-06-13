#!/bin/bash
echo 'Updating our npm packages...'
rm -rf node_modules/mobile-center-link-scripts
rm -rf node_modules/mobile-center-crashes
rm -rf node_modules/mobile-center-analytics
rm -rf node_modules/mobile-center-push
npm install
mkdir -p ios/Pods/RNMobileCenter
cp -R ../RNMobileCenter/Products/RNMobileCenter ios/Pods/RNMobileCenter
