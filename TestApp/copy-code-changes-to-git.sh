#!/bin/bash
echo 'Copy Java/JS changes made inside node_modules to git...'
cd `dirname $0`
for i in '' '-analytics' '-crashes' '-push'
do
    rsync -r node_modules/mobile-center$i/*.js ../mobile-center$i/
    rsync -r node_modules/mobile-center$i/android ../mobile-center$i
done