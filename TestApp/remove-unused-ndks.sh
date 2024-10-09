#!/bin/bash

if [ -z "$1" ]; then
        echo "There is no NDK_VERSION_FOR_BUILD variable provided"
        return 1
    fi

NDK_VERSION_FOR_BUILD="$1"

# Remove all NDKs except NDK 21 to avoid failures on App Center CI with error:
# > No toolchains found in the NDK toolchains folder for ABI with prefix: arm-linux-androideabi 
echo 'Clean unnessesary NDKs'
cd $NDK_PATH

# Enable extended globbing for using regular expression in rm command.
shopt -s extglob
rm -rf !($NDK_VERSION_FOR_BUILD)

echo 'NDKs after clean:'
ls $NDK_PATH