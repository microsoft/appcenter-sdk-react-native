let Platform = require('react-native').Platform;
let RNMobileCenter = require("react-native").NativeModules.RNMobileCenter;

let MobileCenter = {
    // By design, these constants match both the iOS SDK values in MSContants.h and the standard Android values in android.util.Log
    LogLevelVerbose: 2,      // Logging will be very chatty
    LogLevelDebug: 3,        // Debug information will be logged
    LogLevelInfo: 4,         // Information will be logged
    LogLevelWarning: 5,      // Errors and warnings will be logged
    LogLevelError: 6,        // Errors will be logged
    LogLevelAssert: 7,       // Only critical errors will be logged
    LogLevelNone: 99,        // Logging is disabled


    // async - returns a Promise
    getLogLevel() {
        return RNMobileCenter.getLogLevel();
    },

    // async - returns a Promise
    setLogLevel(logLevel){
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
        // Android ReadableMap doesn't support Date type, so pass Dates as a Map Object to work around that
        if (Platform.OS === 'android') {
            let newProperties = {};
            Object.keys(properties).forEach((key) => {
                let value = properties[key];
                if (value instanceof Date) {
                    newProperties[key] = {
                        // RNDate name should be in sync with the matching Java code
                        "RNDate": value.toISOString()
                    };
                }
                else {
                    newProperties[key] = value;
                }
            });
            properties = newProperties;
        }
        return RNMobileCenter.setCustomProperties(properties);
    }
};

module.exports = MobileCenter;
