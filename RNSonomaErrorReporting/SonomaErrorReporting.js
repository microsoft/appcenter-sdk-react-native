let RNSonomaErrorReporting = require("react-native").NativeModules.RNSonomaErrorReporting;

let SonomaErrorReporting = {
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
        return await RNSonomaErrorReporting.getLastSessionCrashDetails();
    },

    async sendCrashes() {
        await RNSonomaErrorReporting.sendCrashes();
    },

    async ignoreCrashes() {
        await RNSonomaErrorReporting.ignoreCrashes();
    },

    async setTextAttachment(textAttachment) {
        await RNSonomaErrorReporting.setTextAttachment(textAttachment);
    }
};

// Android does not have "isDebuggerAttached" method
if (SonomaErrorReporting && RNSonomaErrorReporting.isDebuggerAttached) {
    SonomaErrorReporting = Object.assign({
        async isDebuggerAttached() {
            return await RNSonomaErrorReporting.isDebuggerAttached();
        },
    }, SonomaErrorReporting);
}

module.exports = SonomaErrorReporting;