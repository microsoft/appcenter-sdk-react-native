#!/bin/bash
set -e
directories=("appcenter" "appcenter-analytics" "appcenter-crashes" "appcenter-link-scripts" "appcenter-push" "DemoApp")
for directory in "${directories[@]}"
do
    cd ${directory}
    rm -f package-lock.json

    # npm need to install ESlint locally.
    # ESlint should be installed locally in order to work reliably. Please make sure ESlint is not installed globally.

    npm install
    npm run lint
    cd ..
done
