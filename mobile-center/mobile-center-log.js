const RNMobileCenter = require('react-native').NativeModules.RNMobileCenter;

function format(tag, msg) {
    return `[${tag}] ${msg}`;
}

const MobileCenterLog = {

    // By design, these constants match both the iOS SDK values in MSContants.h and the standard Android values in android.util.Log
    LogLevelVerbose: 2,      // Logging will be very chatty
    LogLevelDebug: 3,        // Debug information will be logged
    LogLevelInfo: 4,         // Information will be logged
    LogLevelWarning: 5,      // Errors and warnings will be logged
    LogLevelError: 6,        // Errors will be logged
    LogLevelAssert: 7,       // Only critical errors will be logged
    LogLevelNone: 99,        // Logging is disabled

    warn(tag, msg) {
        RNMobileCenter.getLogLevel().then((logLevel) => {
            if (logLevel <= MobileCenterLog.LogLevelWarning) {
                console.warn(format(tag, msg));
            }
        });
    },

    error(tag, msg) {
        RNMobileCenter.getLogLevel().then((logLevel) => {
            if (logLevel <= MobileCenterLog.LogLevelError) {
                console.error(format(tag, msg));
            }
        });
    }
};

module.exports = MobileCenterLog;
