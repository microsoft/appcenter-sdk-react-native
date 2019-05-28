// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const Data = {
    show(message, duration) {
        AppCenterReactNativeData.show(message, duration);
    }
};

module.exports = Data;
