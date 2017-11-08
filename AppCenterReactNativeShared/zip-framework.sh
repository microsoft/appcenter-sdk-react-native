#!/bin/bash
cd Products
zipfilename=AppCenter-SDK-ReactNative-iOS-Pod-0.x.x.zip
if [ -f $zipfilename ] ; then
    rm $zipfilename
    echo "  removed old zip"
fi
cp ../../LICENSE AppCenterReactNativeShared
zip -r $zipfilename AppCenterReactNativeShared
echo "output is here: Products/$zipfilename"
