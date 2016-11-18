#!/bin/bash
rm -rf node_modules/mobilecenter-crashes
rm -rf node_modules/mobilecenter-analytics
npm install
react-native link mobilecenter-crashes
react-native link mobilecenter-analytics
