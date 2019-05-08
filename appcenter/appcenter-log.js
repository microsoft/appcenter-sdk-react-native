// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNative } = ReactNative.NativeModules;

function format(tag, msg) {
    return `[${tag}] ${msg}`;
}

const AppCenterLog = {
    LogLevelVerbose: 2,
    LogLevelDebug: 3,
    LogLevelInfo: 4,
    LogLevelWarning: 5,
    LogLevelError: 6,
    LogLevelAssert: 7,
    LogLevelNone: 99,

    warn(tag, msg) {
        AppCenterReactNative.getLogLevel().then((logLevel) => {
            if (logLevel <= AppCenterLog.LogLevelWarning) {
                console.warn(format(tag, msg));
            }
        });
    },

    error(tag, msg) {
        AppCenterReactNative.getLogLevel().then((logLevel) => {
            if (logLevel <= AppCenterLog.LogLevelError) {
                console.error(format(tag, msg));
            }
        });
    }
};

module.exports = AppCenterLog;
