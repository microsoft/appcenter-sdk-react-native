#!/bin/bash
echo 'Installing our npm packages...'
npm install mobile-center-analytics --save
npm install mobile-center-crashes --save
echo 'Linking...'
react-native link mobile-center-analytics
react-native link mobile-center-crashes
