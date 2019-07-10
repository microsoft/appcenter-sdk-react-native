#!/bin/bash

# Build AppCenterReactNativeShared.framework
(cd $SRCROOT; $SRCROOT/build-fat-framework.sh)

# Generate AppCenter-SDK-ReactNative-iOS-Pod-{version}.zip
podspecFile=local.podspec $SRCROOT/../zip-framework.sh
