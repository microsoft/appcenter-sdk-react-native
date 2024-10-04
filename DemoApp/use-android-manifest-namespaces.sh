#!/bin/bash

move_namespace() {
    if [ -z "$1" ]; then
        echo "There is no android project path: move_namespace /path/to/your/android/project"
        return 1
    fi

    PROJECT_PATH="$1"

    BUILD_GRADLE="$PROJECT_PATH/android/build.gradle"
    MANIFEST_XML="$PROJECT_PATH/android/src/main/AndroidManifest.xml"

    if [ ! -f "$BUILD_GRADLE" ]; then
        echo "Error: build.gradle not found: $BUILD_GRADLE"
        return 1
    fi

    if [ ! -f "$MANIFEST_XML" ]; then
        echo "Error: AndroidManifest.xml not found: $MANIFEST_XML"
        return 1
    fi

    NAMESPACE=$(grep -E '^\s*namespace\s*[= ]\s*["'"'"']([^"'"'"']+)["'"'"']' "$BUILD_GRADLE" | head -n 1 | awk -F'"' '{print $2}')
    ESCAPED_NAMESPACE="${NAMESPACE//./\\.}"

    if [ -z "$NAMESPACE" ]; then
        echo "Error: Can not find namespace in build.gradle"
        return 1
    fi

    echo "Founded namespace: $NAMESPACE"

    OS_TYPE=$(uname)
    if [[ "$OS_TYPE" == "Darwin" ]]; then
        # macOS
        SED_INPLACE=("sed" "-i" "")
    else
        # Linux and others
        SED_INPLACE=("sed" "-i")
    fi

    "${SED_INPLACE[@]}" "/namespace \"${ESCAPED_NAMESPACE}\"/d" "$BUILD_GRADLE"
    if grep -q "^\s*namespace\s* \s*['\"]${NAMESPACE}['\"]" "$BUILD_GRADLE"; then
        echo "Error: namespace is not deleted from build.gradle"
        return 1
    else
        echo "Namespace is deleted from build.gradle"
    fi

    CURRENT_PACKAGE=$(grep -E '<manifest\s+package="([^"]+)"' "$MANIFEST_XML" | head -n1 | sed -E 's/.*package="([^"]+)".*/\1/')

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

move_namespace ./node_modules/appcenter
move_namespace ./node_modules/appcenter-analytics
move_namespace ./node_modules/appcenter-crashes
