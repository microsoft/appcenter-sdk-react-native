const ReactNative = require('react-native');

const { AppCenterReactNative } = ReactNative.NativeModules;
const AppCenterLog = require('appcenter/appcenter-log');
const PackageJson = require('./package.json');

const logTag = 'AppCenter';

const AppCenter = {

    // By design, these constants match both the iOS SDK values in MSContants.h and the standard Android values in android.util.Log
    LogLevelVerbose: AppCenterLog.LogLevelVerbose,  // Logging will be very chatty
    LogLevelDebug: AppCenterLog.LogLevelDebug,      // Debug information will be logged
    LogLevelInfo: AppCenterLog.LogLevelInfo,        // Information will be logged
    LogLevelWarning: AppCenterLog.LogLevelWarning,  // Errors and warnings will be logged
    LogLevelError: AppCenterLog.LogLevelError,      // Errors will be logged
    LogLevelAssert: AppCenterLog.LogLevelAssert,    // Only critical errors will be logged
    LogLevelNone: AppCenterLog.LogLevelNone,        // Logging is disabled

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
