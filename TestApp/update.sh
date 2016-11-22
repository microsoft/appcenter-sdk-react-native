#!/bin/bash
echo 'Updating our npm packages...'
rm -rf node_modules/mobilecenter-crashes
rm -rf node_modules/mobilecenter-analytics
npm install
echo 'Unlinking...'
react-native unlink mobilecenter-crashes
react-native unlink mobilecenter-analytics
echo 'Linking...'
react-native link mobilecenter-crashes
react-native link mobilecenter-analytics
