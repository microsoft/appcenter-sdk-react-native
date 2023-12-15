#!/bin/bash
cat >> AppCenterReactNativeShared/android/local.properties << EOL
maven.user=${1:-$MAVEN_USER}
maven.key=${2:-$MAVEN_KEY}
maven.signingKeyId=${3:-$GDP_SIGNING_KEY_ID}
maven.secretKeyPath=${4:-$GDP_KEY_SECRET_PATH}
maven.publicKeyPassword=${5:-$GDP_KEY_PASSWORD}
EOL
