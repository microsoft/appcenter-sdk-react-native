#!/bin/bash
cd Products
zipfilename=MobileCenter-SDK-ReactNative-iOS-Pod-0.x.x.zip
if [ -f $zipfilename ] ; then
    rm $zipfilename
    echo "  removed old zip"
fi
cp ../../LICENSE RNMobileCenterShared
zip -r $zipfilename RNMobileCenterShared
echo "output is here: Products/$zipfilename"
