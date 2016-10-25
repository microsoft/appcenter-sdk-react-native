import { DeviceEventEmitter, NativeModules } from 'react-native';
const RNSonomaCrashes = NativeModules.RNSonomaCrashes;

const willSendEvent = "SonamaErrorReportOnBeforeSending";
const sendDidSucceed = "SonamaErrorReportOnSendingSucceeded";
const sendDidFail = "SonamaErrorReportOnSendingFailed";

let SonomaCrashes = {
    // Constants
    hasCrashedInLastSession: RNSonomaCrashes.hasCrashedInLastSession,
    lastSessionCrashReport: RNSonomaCrashes.lastCrashReport,

    // Functions
    async generateTestCrash() {
        return await RNSonomaCrashes.generateTestCrash();
    },

    async isEnabled() {
        return await RNSonomaCrashes.isEnabled();
    },

    async setEnabled(shouldEnable) {
        await RNSonomaCrashes.setEnabled(shouldEnable);
    },

    process(callback) {
        return RNSonomaCrashes.getCrashReports().then(function (reports) {
            let errorAttachments = {};
            let reportsWithAttachmentFunction = reports.map(function (report) {
                function addAttachment(attachment) {
                    if (typeof attachment != "string") {
                    throw new Error("Only string attachments are supported, received " + typeof attachment);
                    }
                    errorAttachments[report.id] = attachment;
                }
                return Object.assign({
                    addAttachment
                    }, report);
                });
            callback(reportsWithAttachmentFunction, function (response) {
                RNSonomaCrashes.crashUserResponse(response, errorAttachments);
            });
	    });
    },

    addEventListener(listenerMap) {
        if (listenerMap.willSendCrash) {
            DeviceEventEmitter.addListener(willSendEvent, listenerMap.willSendCrash);
        }
        if (listenerMap.didSendCrash) {
            DeviceEventEmitter.addListener(sendDidSucceed, listenerMap.didSendCrash);
        }
        if (listenerMap.failedSendingCrash) {
            DeviceEventEmitter.addListener(sendDidFail, listenerMap.failedSendingCrash);
        }
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