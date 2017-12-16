#!/bin/bash
cat >> local.properties << EOL
bintray.user=${1:-$BINTRAY_USER}
bintray.key=${2:-$BINTRAY_KEY}
bintray.repo=${3:-$BINTRAY_REPO}
bintray.user.org=${4:-$BINTRAY_USER_ORG}
EOL
