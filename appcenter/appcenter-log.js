const ReactNative = require('react-native');

const { AppCenterReactNative } = ReactNative.NativeModules;

function format(tag, msg) {
    return `[${tag}] ${msg}`;
};

const LogLevel = {

    // By design, these constants match both the iOS SDK values in MSContants.h and the standard Android values in android.util.Log
    VERBOSE: 2,      // Logging will be very chatty
    DEBUG: 3,        // Debug information will be logged
    INFO: 4,         // Information will be logged
    WARNING: 5,      // Errors and warnings will be logged
    ERROR: 6,        // Errors will be logged
    ASSERT: 7,       // Only critical errors will be logged
    NONE: 99,        // Logging is disabled
};

const AppCenterLog = {

    /**
     * @deprecated Use `LogLevel.VERBOSE` instead.
     */
    LogLevelVerbose: 2,

    /**
     * @deprecated Use `LogLevel.DEBUG` instead.
     */
    LogLevelDebug: 3,

    /**
     * @deprecated Use `LogLevel.INFO` instead.
     */
    LogLevelInfo: 4,

    /**
     * @deprecated Use `LogLevel.WARNING` instead.
     */
    LogLevelWarning: 5,

    /**
     * @deprecated Use `LogLevel.ERROR` instead.
     */
    LogLevelError: 6,

    /**
     * @deprecated Use `LogLevel.ASSERT` instead.
     */
    LogLevelAssert: 7,

    /**
     * @deprecated Use `LogLevel.NONE` instead.
     */
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
