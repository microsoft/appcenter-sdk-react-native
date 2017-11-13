#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
(cd ../../.. && ./build.sh -t=externals-android)
echo $GOOGLE_SERVICES_JSON | base64 -D > google-services.json
