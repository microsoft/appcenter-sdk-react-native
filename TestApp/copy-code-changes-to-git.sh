#!/bin/bash
echo 'Copy SDK code changes made inside node_modules to git...'
cd `dirname $0`
for i in '' '-analytics' '-crashes' '-push'
do
    for j in '*.js' 'android' 'ios' 'scripts'
    do
        rsync -r node_modules/mobile-center$i/$j ../mobile-center$i
    done
done
rsync -r node_modules/mobile-center-link-scripts/src ../mobile-center-link-scripts
