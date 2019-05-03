# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

# Define default arguments.
MANDATORY=""
GROUP=""
ENVIRONMENT=""
PLATFORM=""

# NOTE: For "--group", use '_' instead of spaces.
for i in "$@"; do
    case $1 in
        -m|--mandatory) MANDATORY="-Mandatory=true" ;;
        -e|--environment) ENVIRONMENT="-Environment=$2"; shift ;;
        -p|--platform) PLATFORM="-Platform=$2"; shift ;;
        -g|--group) GROUP="-Group=$2"; shift ;;
    *) shift ;;
    esac
    shift
done

./build.sh -s test-tools.cake -t="ReleaseApplication" $GROUP $MANDATORY $PLATFORM $ENVIRONMENT
