#!/bin/bash
set -e

npm uninstall --save appcenter appcenter-analytics appcenter-auth appcenter-crashes appcenter-push

npm install --save appcenter appcenter-analytics appcenter-auth appcenter-crashes appcenter-push

pod repo update

react-native link
