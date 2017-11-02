#!/bin/bash
echo 'Updating our npm packages...'
rm -rf node_modules/appcenter-link-scripts
rm -rf node_modules/appcenter
rm -rf node_modules/appcenter-crashes
rm -rf node_modules/appcenter-analytics
rm -rf node_modules/appcenter-push
npm install
