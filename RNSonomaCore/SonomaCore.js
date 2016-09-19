let RNSonomaCore = require("react-native").NativeModules.RNSonomaCore;

const SonomaCore = {
    async setEnabled(isEnabled) {
        await RNSonomaHub.setEnabled(isEnabled);
    },

    async isEnabled() {
        return await RNSonomaCore.isEnabled();
    },

    async getLogLevel() {
        return await RNSonomaCore.getLogLevel();
    },

    async setLogLevel(logLevel) {
        await RNSonomaCore.setLogLevel(logLevel);
    },

    async getInstallId() {
        return await RNSonomaCore.getInstallId();
    },

    LogLevel: {
        ASSERT: RNSonomaCore.LogLevelAssert,
        ERROR: RNSonomaCore.LogLevelError,
        WARNING: RNSonomaCore.LogLevelWarning,
        DEBUG: RNSonomaCore.LogLevelDebug,
        VERBOSE: RNSonomaCore.LogLevelVerbose
    }
};

// Android does not have "NONE" log level
if (RNSonomaCore && RNSonomaCore.LogLevelNone) {
    SonomaCore.LogLevel.NONE = RNSonomaCore.LogLevelNone;
}

module.exports = SonomaCore;