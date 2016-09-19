let RNSonomaCrashes = require("react-native").NativeModules.RNSonomaCrashes;

let SonomaCrashes = {
    async generateTestCrash() {
        await RNSonomaCrashes.generateTestCrash();
    },

    async hasCrashedInLastSession() {
        return await RNSonomaCrashes.hasCrashedInLastSession();
    },

    async notifyWithUserConfirmation(userConfirmation) {
        await RNSonomaCrashes.notifyWithUserConfirmation(userConfirmation);
    },

    async getLastSessionCrashDetails() {
        return await RNSonomaCrashes.getLastSessionCrashDetails();
    },

    async sendCrashes() {
        await RNSonomaCrashes.sendCrashes();
    },

    async ignoreCrashes() {
        await RNSonomaCrashes.ignoreCrashes();
    },

    async setTextAttachment(textAttachment) {
        await RNSonomaCrashes.setTextAttachment(textAttachment);
    }
};

// Android does not have "isDebuggerAttached" method
if (SonomaCrashes && RNSonomaCrashes.isDebuggerAttached) {
    SonomaCrashes = Object.assign({
        async isDebuggerAttached() {
            return await RNSonomaCrashes.isDebuggerAttached();
        },
    }, SonomaCrashes);
}

module.exports = SonomaCrashes;