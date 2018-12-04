#!/bin/bash
set -e

echo 'Removing existing appcenter* packages...'
rm -rf node_modules/appcenter*

echo "Packing appcenter* packages..."
npm pack ../appcenter
npm pack ../appcenter-analytics
npm pack ../appcenter-crashes
npm pack ../appcenter-push
npm pack ../appcenter-link-scripts

echo "Installing appcenter* packages..."
npm install appcenter*.tgz

echo "Cleanup appcenter*.tgz"
rm appcenter*.tgz

echo "Installing other packages..."
npm install

echo "Build shared framework..."
(cd ../AppCenterReactNativeShared/ios && SRCROOT=`pwd` ./build-fat-framework.sh)
