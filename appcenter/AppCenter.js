// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNative } = ReactNative.NativeModules;
const AppCenterLog = require('appcenter/appcenter-log');
const PackageJson = require('./package.json');

const AppCenter = {
    LogLevel: {
        VERBOSE: AppCenterLog.LogLevelVerbose,     // Logging will be very chatty
        DEBUG: AppCenterLog.LogLevelDebug,         // Debug information will be logged
        INFO: AppCenterLog.LogLevelInfo,           // Information will be logged
        WARNING: AppCenterLog.LogLevelWarning,     // Errors and warnings will be logged
        ERROR: AppCenterLog.LogLevelError,         // Errors will be logged
        ASSERT: AppCenterLog.LogLevelAssert,       // Only critical errors will be logged
        NONE: AppCenterLog.LogLevelNone            // Logging is disabled
    },

    /**
     * @deprecated Use `LogLevel.VERBOSE` instead.
     */
    LogLevelVerbose: AppCenterLog.LogLevelVerbose,

    /**
     * @deprecated Use `LogLevel.DEBUG` instead.
     */
    LogLevelDebug: AppCenterLog.LogLevelDebug,

    /**
     * @deprecated Use `LogLevel.INFO` instead.
     */
    LogLevelInfo: AppCenterLog.LogLevelInfo,

    /**
     * @deprecated Use `LogLevel.WARNING` instead.
     */
    LogLevelWarning: AppCenterLog.LogLevelWarning,

    /**
     * @deprecated Use `LogLevel.ERROR` instead.
     */
    LogLevelError: AppCenterLog.LogLevelError,

    /**
     * @deprecated Use `LogLevel.ASSERT` instead.
     */
    LogLevelAssert: AppCenterLog.LogLevelAssert,

    /**
     * @deprecated Use `LogLevel.NONE` instead.
     */
    LogLevelNone: AppCenterLog.LogLevelNone,

    // async - returns a Promise
    startFromLibrary(service) {
        return AppCenterReactNative.startFromLibrary(service);
    },

    // async - returns a Promise
    getLogLevel() {
        return AppCenterReactNative.getLogLevel();
    },

    // async - returns a Promise
    setLogLevel(logLevel) {
        return AppCenterReactNative.setLogLevel(logLevel);
    },

    // async - returns a Promise
    setUserId(userId) {
        return AppCenterReactNative.setUserId(userId);
    },

    // async - returns a Promise
    getInstallId() {
        return AppCenterReactNative.getInstallId();
    },

    // async - returns a Promise
    isEnabled() {
        return AppCenterReactNative.isEnabled();
    },

    // async - returns a Promise
    setEnabled(enabled) {
        return AppCenterReactNative.setEnabled(enabled);
    },

    // async - returns a Promise
    isNetworkRequestsAllowed() {
        return AppCenterReactNative.isNetworkRequestsAllowed();
    },

    // async - returns a Promise
    setNetworkRequestsAllowed(isAllowed) {
        return AppCenterReactNative.setNetworkRequestsAllowed(isAllowed);
    },

    getSdkVersion() {
        return PackageJson.version;
    }
};

module.exports = AppCenter;
