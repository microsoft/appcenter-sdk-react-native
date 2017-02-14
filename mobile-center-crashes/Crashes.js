let ReactNative = require('react-native');
const RNCrashes = ReactNative.NativeModules.RNCrashes;

const willSendEvent = "MobileCenterErrorReportOnBeforeSending";
const sendDidSucceed = "MobileCenterErrorReportOnSendingSucceeded";
const sendDidFail = "MobileCenterErrorReportOnSendingFailed";

let Crashes = {
    // async - returns a Promise
    generateTestCrash() {
        return RNCrashes.generateTestCrash();
    },

    // async - returns a Promise
    hasCrashedInLastSession() {
        return RNCrashes.hasCrashedInLastSession();
    },

    // async - returns a Promise
    lastSessionCrashReport() {
        return RNCrashes.lastSessionCrashReport();
    },

    // async - returns a Promise
    isEnabled() {
        return RNCrashes.isEnabled();
    },

    // async - returns a Promise
    setEnabled(shouldEnable) {
        return RNCrashes.setEnabled(shouldEnable);
    },

    process(callback) {
        return RNCrashes.getCrashReports().then(function (reports) {
            let errorAttachments = {};
            /* TODO: Re-enable error attachment when the feature becomes available.
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
            reports = reportsWithAttachmentFunction;
            */
            
            callback(reports, function (response) {
                RNCrashes.crashUserResponse(response, errorAttachments);
            });
	    });
    },

    addEventListener(listenerMap) {
        if (listenerMap.willSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(willSendEvent, listenerMap.willSendCrash);
        }
        if (listenerMap.didSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(sendDidSucceed, listenerMap.didSendCrash);
        }
        if (listenerMap.failedSendingCrash) {
            ReactNative.DeviceEventEmitter.addListener(sendDidFail, listenerMap.failedSendingCrash);
        }
    }
};

// Android does not have "isDebuggerAttached" method
if (Crashes && RNCrashes && RNCrashes.isDebuggerAttached) {
    Crashes = Object.assign({
        // async - returns a Promise
        isDebuggerAttached() {
            return RNCrashes.isDebuggerAttached();
        },
    }, Crashes);
}

module.exports = Crashes;
