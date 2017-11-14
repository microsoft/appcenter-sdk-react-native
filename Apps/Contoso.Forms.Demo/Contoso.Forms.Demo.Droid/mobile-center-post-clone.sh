#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
sed -i '' "s/NUGET_PASSWORD/$NUGET_PASSWORD/g" ../../../NuGet.config
echo $GOOGLE_SERVICES_JSON | base64 -D > google-services.json
(cd ../../.. && ./build.sh -t=externals-android)
