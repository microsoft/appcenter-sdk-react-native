let RNSonomaCrashes = require("react-native").NativeModules.RNSonomaCrashes;

function deserializeCrashReport(nativeReport) {
    if (!nativeReport) {
	return;
    }

    let errorReport = {};
    errorReport.id = nativeReport.id;
    errorReport.threadName = nativeReport.threadName;
    return errorReport;
}

let SonomaCrashes = {
    // Constants
    hasCrashedInLastSession: RNSonomaCrashes.hasCrashedInLastSession,
    lastSessionCrashReport: deserializeCrashReport(RNSonomaCrashes.lastError),

    // Functions
    async generateTestCrash() {
        await RNSonomaCrashes.generateTestCrash();
    },

    async sendCrash() {
        await RNSonomaCrashes.sendCrash();
    },

    async ignoreCrash() {
        await RNSonomaCrashes.ignoreCrash();
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