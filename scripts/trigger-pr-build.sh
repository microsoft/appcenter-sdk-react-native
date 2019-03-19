#!/bin/bash

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

set -e

build_id=`curl -s -u $VSTS_USER:$VSTS_PASSWORD \
  -H "Content-Type: application/json" \
  $VSTS_PROJECT_URL/_apis/build/builds?api-version=2.0 -d \
  "{
    'definition': {
      'id': $VSTS_BUILD_DEFINITION
    },\
    'sourceBranch': 'refs/pull/$BITRISE_PULL_REQUEST/merge'
  }" | jq -r .id`

build_result="null"
while [ "$build_result" == "null" ]
do
  echo "Polling build status ID=$build_id ..."
  build_result=`curl -s -u $VSTS_USER:$VSTS_PASSWORD \
    -H "Content-Type: application/json" \
    $VSTS_PROJECT_URL/_apis/build/builds/$build_id?api-version=2.0 | jq -r '.result'`
  if [ "$build_result" == "null" ]
  then
    sleep 10
  fi
done

echo "Build completed with result: $build_result"

if [ "$build_result" != "succeeded" ]
then
  exit 1
fi
