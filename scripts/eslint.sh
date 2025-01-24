#!/bin/bash
set -e

# ESLint should be installed locally in order to work reliably.
# Please make sure ESLint is only installed locally not globally.

directories=("appcenter" "appcenter-analytics" "appcenter-crashes" "appcenter-link-scripts" "TestApp")
for directory in "${directories[@]}"
do
    cd ${directory}
    rm -f package-lock.json
    if [ "$directory" = "TestApp" ]; then
        ./prepare-local-sdk-integration.sh
    else
        npm install
    fi
    npm run lint
    cd ..
done
