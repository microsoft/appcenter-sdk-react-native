#!/bin/bash
cd Products
zipfilename=MobileCenter-SDK-ReactNative-iOS-Pod-0.x.x.zip
if [ -f $zipfilename ] ; then
    rm $zipfilename
    echo "  removed old zip"
fi
cp ../../LICENSE RNMobileCenter
zip -r $zipfilename RNMobileCenter
echo "output is here: Products/$zipfilename"
