#!/bin/bash

# Build AppCenterReactNativeShared.framework
(cd $SRCROOT; $SRCROOT/build-xcframework.sh)

# Generate AppCenter-SDK-ReactNative-iOS-Pod-{version}.zip
podspecFile=local.podspec $SRCROOT/../zip-framework.sh
