#!/bin/bash
set -e

echo "Packing appcenter* packages..."
npm pack ./appcenter
npm pack ./appcenter-analytics
npm pack ./appcenter-auth
npm pack ./appcenter-crashes
npm pack ./appcenter-push
npm pack ./appcenter-link-scripts

echo "Move appcenter*.tgz into sub-directory ./packages"
mkdir -p packages
cp appcenter*.tgz ./packages
rm appcenter*.tgz
