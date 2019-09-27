#!/bin/bash
set -e

npm uninstall --save appcenter appcenter-analytics appcenter-auth appcenter-crashes appcenter-push appcenter-data

npm install --save appcenter appcenter-analytics appcenter-auth appcenter-crashes appcenter-push appcenter-data

pod repo update

(cd ios && pod install)

react-native link
