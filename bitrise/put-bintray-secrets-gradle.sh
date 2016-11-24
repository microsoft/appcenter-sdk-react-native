#!/bin/bash
set -e
echo "bintray.user=$BINTRAY_USER" > local.properties
echo "bintray.key=$BINTRAY_KEY" >> local.properties
echo "bintray.user.org=$BINTRAY_USER_ORG" >> local.properties
echo "bintray.repo=$BINTRAY_REPO" >> local.properties
