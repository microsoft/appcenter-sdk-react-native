// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const Data = {
    read(documentId, partition) {
        return AppCenterReactNativeData.read(documentId, partition);
    }
};

module.exports = Data;
