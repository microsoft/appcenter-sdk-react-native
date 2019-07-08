#!/bin/bash
(cd $SRCROOT; $SRCROOT/build-fat-framework.sh)
podspecFile=local.podspec $SRCROOT/../zip-framework.sh