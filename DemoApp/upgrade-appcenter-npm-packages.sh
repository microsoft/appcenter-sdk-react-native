#!/bin/bash
set -e

npm uninstall --save appcenter appcenter-analytics appcenter-crashes

npm install --save appcenter appcenter-analytics appcenter-crashes

pod repo update

(cd ios && pod install)

npx react-native link
