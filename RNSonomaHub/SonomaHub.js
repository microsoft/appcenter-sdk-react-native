let RNSonomaHub = require("react-native").NativeModules.RNSonomaHub;

module.exports = {
    async setEnabled(isEnabled) {
        await RNSonomaHub.setEnabled(isEnabled);
    },

    async isEnabled() {
        return await RNSonomaHub.isEnabled();
    },

    async getLogLevel() {
        return await RNSonomaHub.logLevel();
    },

    async setLogLevel(logLevel) {
        await RNSonomaHub.setLogLevel(logLevel);
    },

    async getInstallId() {
        return await RNSonomaHub.installId();
    },

    LogLevel: {
        NONE: RNSonomaHub.AVALogLevelNone,
        ERROR: RNSonomaHub.AVALogLevelError,
        WARNING: RNSonomaHub.AVALogLevelWarning,
        DEBUG: RNSonomaHub.AVALogLevelDebug,
        VERBOSE: RNSonomaHub.AVALogLevelVerbose
    }
};