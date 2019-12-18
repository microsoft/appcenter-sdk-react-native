// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const ReactNative = require('react-native');

const { AppCenterReactNative } = ReactNative.NativeModules;
const AppCenterLog = require('appcenter/appcenter-log');
const PackageJson = require('./package.json');

const logTag = 'AppCenter';

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
    setCustomProperties(properties) {
        if (properties instanceof AppCenter.CustomProperties) {
            return AppCenterReactNative.setCustomProperties(properties);
        }
        const type = Object.prototype.toString.apply(properties);
        AppCenterLog.error(logTag, `SetCustomProperties: Invalid type, expected CustomProperties but got ${type}.`);
        return Promise.resolve(null);
    },

    getSdkVersion() {
        return PackageJson.version;
    }
};

AppCenter.CustomProperties = class {
    set(key, value) {
        if (typeof key === 'string') {
            const type = typeof value;
            switch (type) {
                case 'string':
                case 'number':
                case 'boolean':
                    this[key] = { type, value };
                    break;

                case 'object':
                    if (value instanceof Date) {
                        this[key] = { type: 'date-time', value: value.getTime() };
                    } else {
                        AppCenterLog.error(logTag, 'CustomProperties: Invalid value type, expected string|number|boolean|Date.');
                    }
                    break;

                default:
                    AppCenterLog.error(logTag, 'CustomProperties: Invalid value type, expected string|number|boolean|Date.');
            }
        } else {
            AppCenterLog.error(logTag, 'CustomProperties: Invalid key type, expected string.');
        }
        return this;
    }

    clear(key) {
        if (typeof key === 'string') {
            this[key] = { type: 'clear' };
        } else {
            AppCenterLog.error(logTag, 'CustomProperties: Invalid key type, expected string.');
        }
        return this;
    }
};

module.exports = AppCenter;
