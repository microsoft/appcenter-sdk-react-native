#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Move android namespaces for the AppCenter modules from build.gradle to AndroidManifest.xml

move_namespace() {
    # Check if the first argument is provided
    if [ -z "$1" ]; then
        echo "There is no android project path: move_namespace /path/to/your/android/project"
        return 1
    fi

    PROJECT_PATH="$1"
    BUILD_GRADLE="$PROJECT_PATH/android/build.gradle"
    MANIFEST_XML="$PROJECT_PATH/android/src/main/AndroidManifest.xml"

    # Verify if build.gradle exists
    if [ ! -f "$BUILD_GRADLE" ]; then
        echo "Error: build.gradle not found: $BUILD_GRADLE"
        return 1
    fi

    # Verify if AndroidManifest.xml exists
    if [ ! -f "$MANIFEST_XML" ]; then
        echo "Error: AndroidManifest.xml not found: $MANIFEST_XML"
        return 1
    fi

    # Extract namespace from build.gradle
    NAMESPACE=$(grep -E '^\s*namespace\s*[= ]\s*["'"'"']([^"'"'"']+)["'"'"']' "$BUILD_GRADLE" | head -n 1 | awk -F'"' '{print $2}')
    ESCAPED_NAMESPACE="${NAMESPACE//./\\.}"

    if [ -z "$NAMESPACE" ]; then
        echo "Error: Can not find namespace in build.gradle"
        return 1
    fi

    echo "Founded namespace: $NAMESPACE"

    # Determine the operating system type and configure sed command for inline editing
    OS_TYPE=$(uname)
    if [[ "$OS_TYPE" == "Darwin" ]]; then
        # macOS
        SED_INPLACE=("sed" "-i" "")
    else
        # Linux and others
        SED_INPLACE=("sed" "-i")
    fi

    # Remove the namespace line from build.gradle
    "${SED_INPLACE[@]}" "/namespace \"${ESCAPED_NAMESPACE}\"/d" "$BUILD_GRADLE"
    # Check if the namespace is successfully deleted
    if grep -q "^\s*namespace\s* \s*['\"]${NAMESPACE}['\"]" "$BUILD_GRADLE"; then
        echo "Error: namespace is not deleted from build.gradle"
        return 1
    else
        echo "Namespace is deleted from build.gradle"
    fi

    # Extract the current package name from AndroidManifest.xml
    CURRENT_PACKAGE=$(grep -E '<manifest\s+package="([^"]+)"' "$MANIFEST_XML" | head -n1 | sed -E 's/.*package="([^"]+)".*/\1/')

    # Update or add the package attribute in AndroidManifest.xml if it differs from the namespace
    if [ "$CURRENT_PACKAGE" != "$NAMESPACE" ]; then
        if grep -q 'package="' "$MANIFEST_XML"; then
            "${SED_INPLACE[@]}" "s/package=\"[^\"]*\"/package=\"$NAMESPACE\"/" "$MANIFEST_XML"
            echo "Updated package attribute in AndroidManifest.xml to $NAMESPACE"
        else
            "${SED_INPLACE[@]}" "1s/<manifest /<manifest package=\"$NAMESPACE\" /" "$MANIFEST_XML"
            echo "Added attribute package to AndroidManifest.xml: $NAMESPACE"
        fi
    else
        echo "Package attribute already matches namespace: $NAMESPACE"
    fi

    echo "Namespace successfully moved from $BUILD_GRADLE to $MANIFEST_XML"
    return 0
}

# Move namespaces for the AppCenter modules
move_namespace ./node_modules/appcenter
move_namespace ./node_modules/appcenter-analytics
move_namespace ./node_modules/appcenter-crashes
