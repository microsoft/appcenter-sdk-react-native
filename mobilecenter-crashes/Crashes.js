import { DeviceEventEmitter, NativeModules } from 'react-native';
const RNCrashes = NativeModules.RNCrashes;

const willSendEvent = "MobileCenterErrorReportOnBeforeSending";
const sendDidSucceed = "MobileCenterErrorReportOnSendingSucceeded";
const sendDidFail = "MobileCenterErrorReportOnSendingFailed";

let Crashes = {
    // Constants
    hasCrashedInLastSession: RNCrashes.hasCrashedInLastSession,
    lastSessionCrashReport: RNCrashes.lastCrashReport,

    // Functions
    async generateTestCrash() {
        return await RNCrashes.generateTestCrash();
    },

    async isEnabled() {
        return await RNCrashes.isEnabled();
    },

    async setEnabled(shouldEnable) {
        await RNCrashes.setEnabled(shouldEnable);
    },

    process(callback) {
        return RNCrashes.getCrashReports().then(function (reports) {
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
                RNCrashes.crashUserResponse(response, errorAttachments);
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
if (Crashes && RNCrashes.isDebuggerAttached) {
    Crashes = Object.assign({
        async isDebuggerAttached() {
            return await RNCrashes.isDebuggerAttached();
        },
    }, Crashes);
}

module.exports = Crashes;