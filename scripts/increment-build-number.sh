#!/bin/bash
set -e

# Extract semantic version template from Core PCL project.
semanticVersion=`grep AssemblyInformationalVersion Microsoft.Sonoma.Core/Properties/AssemblyInfo.cs | sed -E "s/^.*\"(.*)\".*$/\1/"`
baseSemanticVersion=`sed -E 's/(.*)-.*/\1/' <<< "$semanticVersion"`

newRevision=123 #TODO dynamic by calling get latest version on nuget repos

# Nuget/cake does not support semver 2.0.0, work around with 1.0.0 limitations
# pad with zeroes and add a letter before as cake will fail for some reason
# if it starts with a number (supposed to work even in semver 1.0.0).
# r stands for revision
newRevisionPadded=`printf %04d $newRevision`
newVersion="${baseSemanticVersion}-r${newRevisionPadded}"

# If the build is automated, we don't create a git tag, so add build metada to version to be able to know where that build comes from (git commit abbreviated).
if [[ "$1" == "--use-hash" ]]
then

    # Again in semver 2.0.0 its supposed to be a + sign, not a dash, but nuget/cake is 1.0.0
    newVersion+="-$(git rev-parse --short HEAD)"
fi

# Replace version in all the assemblies
find . -name AssemblyInfo.cs | xargs sed -E -i '' "s/(AssemblyInformationalVersion\(\")(.*)\"\)/\1$newVersion\")/g"
find . -name AssemblyInfo.cs | xargs sed -E -i '' "s/(AssemblyFileVersion\(\"([0-9]+\.){3})(.*)\"\)/\1$newRevision\")/g"
