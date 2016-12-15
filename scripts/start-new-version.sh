#!/bin/bash
set -e

export version=$1
export snapshotVersion=$1-SNAPSHOT

# Replace version in all the assemblies
for file in `find . -name AssemblyInfo.cs | grep -v Demo`
do
    sed -E -i '' "s/(AssemblyInformationalVersion\(\")(.*)\"\)/\1$snapshotVersion\")/g" $file
    sed -E -i '' "s/(AssemblyFileVersion\(\")(.*)\"\)/\1$version.0\")/g" $file
done

sed -E -i '' "s/(Version = \")(.*)\"/\1$snapshotVersion\"/g" SDK/MobileCenter/Microsoft.Azure.Mobile.Shared/WrapperSdk.cs

# Replace android versions
for file in `find . -name AndroidManifest.xml | grep Properties | grep -v Demo`
do
    sed -E -i '' "s/(android:versionName=\")([^\"]+)/\1$snapshotVersion/g" $file
    versionCode=$((`grep versionCode $file | sed -E "s/^.*versionCode=\"([^\"]*)\".*$/\1/"`+1))
    sed -E -i '' "s/(android:versionCode=\")([^\"]+)/\1$versionCode/g" $file
done

# Replace ios versions
for file in `find Apps Tests -name Info.plist | egrep -v "/(obj|bin|.*Demo.*)/"`
do
    perl -pi -e 'undef $/; s/(CFBundleVersion<\/key>\s*<string>)([^<]*)/${1}$ENV{version}/' $file
    perl -pi -e 'undef $/; s/(CFBundleShortVersionString<\/key>\s*<string>)([^<]*)/${1}$ENV{version}/' $file
done