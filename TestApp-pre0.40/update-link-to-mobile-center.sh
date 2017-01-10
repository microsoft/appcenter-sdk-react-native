#!/bin/bash
echo 'Updating our npm packages...'
rm -rf node_modules/mobile-center-link-scripts
rm -rf node_modules/mobile-center-crashes
rm -rf node_modules/mobile-center-analytics
npm install
echo 'Unlinking...'
react-native unlink mobile-center-crashes
react-native unlink mobile-center-analytics
echo 'Linking...'
react-native link mobile-center-crashes
react-native link mobile-center-analytics
