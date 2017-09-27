const RNMobileCenter = require('react-native').NativeModules.RNMobileCenter;
const MobileCenterLog = require('mobile-center/mobile-center-log');
const PackageJson = require('./package.json');

const logTag = 'MobileCenter';

const MobileCenter = {

    // By design, these constants match both the iOS SDK values in MSContants.h and the standard Android values in android.util.Log
    LogLevelVerbose: MobileCenterLog.LogLevelVerbose,  // Logging will be very chatty
    LogLevelDebug: MobileCenterLog.LogLevelDebug,      // Debug information will be logged
    LogLevelInfo: MobileCenterLog.LogLevelInfo,        // Information will be logged
    LogLevelWarning: MobileCenterLog.LogLevelWarning,  // Errors and warnings will be logged
    LogLevelError: MobileCenterLog.LogLevelError,      // Errors will be logged
    LogLevelAssert: MobileCenterLog.LogLevelAssert,    // Only critical errors will be logged
    LogLevelNone: MobileCenterLog.LogLevelNone,        // Logging is disabled

    // async - returns a Promise
    getLogLevel() {
        return RNMobileCenter.getLogLevel();
    },

    // async - returns a Promise
    setLogLevel(logLevel) {
        return RNMobileCenter.setLogLevel(logLevel);
    },

    // async - returns a Promise
    getInstallId() {
        return RNMobileCenter.getInstallId();
    },

    // async - returns a Promise
    isEnabled() {
        return RNMobileCenter.isEnabled();
    },

    // async - returns a Promise
    setEnabled(enabled) {
        return RNMobileCenter.setEnabled(enabled);
    },

    // async - returns a Promise
    setCustomProperties(properties) {
        if (properties instanceof MobileCenter.CustomProperties) {
            return RNMobileCenter.setCustomProperties(properties);
        }
        const type = Object.prototype.toString.apply(properties);
        MobileCenterLog.error(logTag, `SetCustomProperties: Invalid type, expected CustomProperties but got ${type}.`);
        return Promise.reject('Could not set custom properties because of invalid type.');
    },

    getSdkVersion() {
        return PackageJson.version;
    }
};

MobileCenter.CustomProperties = class {
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
                        MobileCenterLog.error(logTag, 'CustomProperties: Invalid value type, expected string|number|boolean|Date.');
                    }
                    break;

                default:
                    MobileCenterLog.error(logTag, 'CustomProperties: Invalid value type, expected string|number|boolean|Date.');
            }
        } else {
            MobileCenterLog.error(logTag, 'CustomProperties: Invalid key type, expected string.');
        }
        return this;
    }

    clear(key) {
        if (typeof key === 'string') {
            this[key] = { type: 'clear' };
        } else {
            MobileCenterLog.error(logTag, 'CustomProperties: Invalid key type, expected string.');
        }
        return this;
    }
};

module.exports = MobileCenter;
