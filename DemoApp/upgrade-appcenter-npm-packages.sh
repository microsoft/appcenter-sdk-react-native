#!/bin/bash
set -e

npm uninstall --save appcenter appcenter-analytics appcenter-crashes appcenter-push

npm install --save appcenter appcenter-analytics appcenter-crashes appcenter-push

pod repo update

react-native link
