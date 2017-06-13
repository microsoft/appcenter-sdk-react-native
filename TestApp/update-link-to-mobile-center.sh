#!/bin/bash
./update-npm-packages.sh
echo 'Unlinking...'
react-native unlink mobile-center-crashes
react-native unlink mobile-center-analytics
react-native unlink mobile-center-push
echo 'Linking...'
react-native link mobile-center-crashes
react-native link mobile-center-analytics
react-native link mobile-center-push
