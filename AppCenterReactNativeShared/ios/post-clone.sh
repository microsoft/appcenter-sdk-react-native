#!/usr/bin/env bash
echo "Update pod repository"
pod repo update
echo "Installing pods for AppCenterReactNativeShared in `pwd`"
pod install
