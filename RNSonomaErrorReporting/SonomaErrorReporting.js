let RNSonomaErrorReporting = require("react-native").NativeModules.RNSonomaErrorReporting;

module.exports = {
    async isDebuggerAttached() {
        return await RNSonomaErrorReporting.isDebuggerAttached();
    },

    async generateTestCrash() {
        await RNSonomaErrorReporting.generateTestCrash();
    },

    async hasCrashedInLastSession() {
        return await RNSonomaErrorReporting.hasCrashedInLastSession();
    },

    async notifyWithUserConfirmation(userConfirmation) {
        await RNSonomaErrorReporting.notifyWithUserConfirmation(userConfirmation);
    },

    async getLastSessionCrashDetails() {
        return await RNSonomaErrorReporting.lastSessionCrashDetails();
    },

    ErrorLogSetting: {
        DISABLED: RNSonomaErrorReporting.AVAErrorLogSettingDisabled,
        ALWAYS_ASK: RNSonomaErrorReporting.AVAErrorLogSettingAlwaysAsk,
        AUTO_SEND: RNSonomaErrorReporting.AVAErrorLogSettingAutoSend
    },

    UserConfirmation: {
        DONT_SEND: RNSonomaErrorReporting.AVAUserConfirmationDontSend,
        SEND: RNSonomaErrorReporting.AVAUserConfirmationSend,
        ALWAYS: RNSonomaErrorReporting.AVAUserConfirmationAlways
    }
};