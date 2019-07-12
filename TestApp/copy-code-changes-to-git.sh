#!/bin/bash
echo 'Copy SDK code changes made inside node_modules to git...'
cd `dirname $0`
for i in '' '-analytics' '-crashes' '-push' '-auth'
do
    for j in '*.js' 'android' 'ios' 'scripts'
    do
        rsync -r --delete node_modules/appcenter$i/$j ../appcenter$i
    done
done
rsync -r node_modules/appcenter-link-scripts/src ../appcenter-link-scripts
