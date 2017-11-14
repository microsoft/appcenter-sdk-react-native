#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
sed -i '' "s/NUGET_PASSWORD/$NUGET_PASSWORD/g" NuGet.config
echo $GOOGLE_SERVICES_JSON | base64 -D > Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/google-services.json
./build.sh -t=externals-android
