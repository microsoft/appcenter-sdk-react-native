let RNSonomaHub = require("react-native").NativeModules.RNSonomaHub;

const SonomaHub = {
    async setEnabled(isEnabled) {
        await RNSonomaHub.setEnabled(isEnabled);
    },

    async isEnabled() {
        return await RNSonomaHub.isEnabled();
    },

    async getLogLevel() {
        return await RNSonomaHub.getLogLevel();
    },

    async setLogLevel(logLevel) {
        await RNSonomaHub.setLogLevel(logLevel);
    },

    async getInstallId() {
        return await RNSonomaHub.getInstallId();
    },

    LogLevel: {
        ASSERT: RNSonomaHub.LogLevelAssert,
        ERROR: RNSonomaHub.LogLevelError,
        WARNING: RNSonomaHub.LogLevelWarning,
        DEBUG: RNSonomaHub.LogLevelDebug,
        VERBOSE: RNSonomaHub.LogLevelVerbose
    }
};

// Android does not have "NONE" log level
if (RNSonomaHub && RNSonomaHub.LogLevelNone) {
    SonomaHub.LogLevel.NONE = RNSonomaHub.LogLevelNone;
}

module.exports = SonomaHub;