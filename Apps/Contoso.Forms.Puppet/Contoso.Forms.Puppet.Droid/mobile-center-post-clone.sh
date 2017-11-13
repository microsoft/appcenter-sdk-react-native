#!/usr/bin/env bash
echo "Executing post clone script in `pwd`"
./build.sh -t=externals-android
echo $GOOGLE_SERVICES_JSON | base64 -D > Apps/Contoso.Forms.Puppet/Contoso.Forms.Puppet.Droid/google-services.json
