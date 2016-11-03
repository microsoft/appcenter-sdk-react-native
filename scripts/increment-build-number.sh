#!/bin/bash
set -e

# Extract semantic version template from Core PCL project.
semanticVersion=`grep AssemblyInformationalVersion Microsoft.Sonoma.Core/Properties/AssemblyInfo.cs | sed -E "s/^.*\"(.*)\".*$/\1/"`
baseSemanticVersion=`sed -E 's/(.*)-.*/\1/' <<< "$semanticVersion"`

# Get latest version from Nuget
latestNugetVersion=`curl -s -H "X-NuGet-ApiKey: $NUGET_PASSWORD" "https://mseng.pkgs.visualstudio.com/_packaging/150e39b1-bf52-4fdd-bc32-28d950a14304/nuget/v2/Search()?\\$filter=IsAbsoluteLatestVersion+and+Id+eq+'Microsoft.Sonoma.Core'&includePrerelease=true" --user $NUGET_USER:$NUGET_PASSWORD`
latestNugetVersion=`sed -E "s/^.*<d:Version>(.*)<\/d:Version>.*$/\1/" <<< $latestNugetVersion`
latestNugetBaseVersion=`sed -E 's/([^-]*)-.*/\1/' <<< "$latestNugetVersion"`
latestNugetRevision=`sed -E 's/^.*-r0*(.*)-.*$/\1/' <<< "$latestNugetVersion"`

# Check if base version changes, the second check is just to check if the revision is valid
# If latestNugetVersion=latestNugetRevision that just means sed failed matching
# e.g. first revision not yet published
if [[ "$baseSemanticVersion" != "$latestNugetBaseVersion" ]] || [[ "$latestNugetVersion" == "$latestNugetRevision" ]]
then
    newRevision=1 # we use 0 padding and regexes, so start revisions at 1 to simplify
else
    newRevision=$((latestNugetRevision+1))
fi

# Nuget/cake does not support semver 2.0.0, work around with 1.0.0 limitations
# pad with zeroes and add a letter before as cake will fail for some reason
# if it starts with a number (supposed to work even in semver 1.0.0).
# r stands for revision
newRevisionPadded=`printf %04d $newRevision`
newVersion="${baseSemanticVersion}-r${newRevisionPadded}"

# If the build is automated, we don't create a git tag, so add build metadata to version to be able to know where that build comes from (git commit abbreviated).
if [[ "$1" == "--use-hash" ]]
then

    # Again in semver 2.0.0 its supposed to be a + sign, not a dash, but nuget/cake is 1.0.0
    newVersion+="-$(git rev-parse --short HEAD)"
fi

# Replace version in all the assemblies
find . -name AssemblyInfo.cs | xargs sed -E -i '' "s/(AssemblyInformationalVersion\(\")(.*)\"\)/\1$newVersion\")/g"
find . -name AssemblyInfo.cs | xargs sed -E -i '' "s/(AssemblyFileVersion\(\"([0-9]+\.){3})(.*)\"\)/\1$newRevision\")/g"
