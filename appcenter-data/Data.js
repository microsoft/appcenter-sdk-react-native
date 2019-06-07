// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const Data = {
    read(documentId, partition) {
        return AppCenterReactNativeData.read(documentId, partition).then(result => {

            // Create a new `Date` object from timestamp as milliseconds
            result.lastUpdatedDate = new Date(result.lastUpdatedDate);
            return result;
        });
    }
};

module.exports = Data;
