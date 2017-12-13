#!/bin/bash
set -e
directories=("appcenter" "appcenter-analytics" "appcenter-crashes" "appcenter-link-scripts" "appcenter-push" "TestApp" "DemoApp")
for directory in "${directories[@]}"
do
    cd ${directory}
    npm run lint
    cd ..
done
