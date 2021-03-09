#!/bin/bash
set -e
# Description: Add a `SNAPSHOT` prefix for `versionName` in `versions.gradle`.
# Usage: ./scripts/add-snapshot-prefix.sh

# Compute new version
gradleVersion=`egrep "versionName '(.*)'" *.gradle | sed -E "s/^.*versionName '(.*)'.*$/\1/"`
baseGradleVersion=`echo $gradleVersion | cut -d'-' -f 1`
newVersion=$baseGradleVersion-"SNAPSHOT"

# Replace version in the file
sed -E -i '' "s#(versionName ')(.*)'#\1$newVersion'#" *.gradle
# Print version on build logs
grep "versionName '" *.gradle