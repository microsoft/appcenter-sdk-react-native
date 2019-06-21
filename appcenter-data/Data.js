// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNativeData } = ReactNative.NativeModules;

const TimeToLive = {
    INFINITE: -1,
    NO_CACHE: 0,
    DEFAULT: 86400
};

const DefaultPartitions = {
    USER_DOCUMENTS = 'user',
    APP_DOCUMENTS = 'readonly'
};

const Data = {
    TimeToLive,
    DefaultPartitions,
    read(documentId, partition, ttl) {
        if(ttl === undefined) {
            ttl = TimeToLive.DEFAULT
        }

        return AppCenterReactNativeData.read(documentId, partition, ttl).then(result => {
            // Create a new `Date` object from timestamp as milliseconds
            result.lastUpdatedDate = new Date(result.lastUpdatedDate);
            return result;
        });
    }
};

module.exports = Data;
