#!/bin/bash
set -e

# Extract semantic version template from Core PCL project.
currentVersion=`grep AssemblyInformationalVersion SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs | sed -E "s/^.*\"(.*)\".*$/\1/"`

# Remove pre-release
officialVersion=`sed -E 's/(.*)-.*/\1/' <<< "$currentVersion"`

# Replace version in all the assemblies
for file in `find . -name AssemblyInfo.cs | grep -v Demo`
do
    sed -E -i '' "s/(AssemblyInformationalVersion\(\")(.*)\"\)/\1$officialVersion\")/g" $file
done

sed -E -i '' "s/(Version = \")(.*)\"/\1$officialVersion\"/g" SDK/MobileCenter/Microsoft.Azure.Mobile.Shared/WrapperSdk.cs

# Replace android versions
for file in `find . -name AndroidManifest.xml | grep Properties | grep -v Demo`
do
    sed -E -i '' "s/(android:versionName=\")([^\"]+)/\1$officialVersion/g" $file
done
