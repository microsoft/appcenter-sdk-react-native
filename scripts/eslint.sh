#!/bin/bash
set -e

# ESLint should be installed locally in order to work reliably.
# Please make sure ESLint is only installed locally not globally.

directories=("appcenter" "appcenter-analytics" "appcenter-auth" "appcenter-crashes" "appcenter-link-scripts" "appcenter-push" "DemoApp" "TestApp")
for directory in "${directories[@]}"
do
    cd ${directory}
    rm -f package-lock.json
    if [ "$directory" = "TestApp" ]; then
        ./update-npm-packages.sh
    else
        npm install
    fi
    npm run lint
    cd ..
done
