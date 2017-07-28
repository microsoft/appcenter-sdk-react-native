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

    async process(callback) {

        // Checking enabled will make sure the callback is executed after the Android SDK has finished loading
        // crash reports in background. We could call getCrashReports too soon otherwise in case
        // it takes a lot of time.
        const enabled = await RNCrashes.isEnabled();
        if (enabled) {
            const reports = await RNCrashes.getCrashReports();
            let errorAttachments = {};
            let reportsWithAttachmentFunction = reports.map(function (report) {
                function addTextAttachment(text, filename) {
                    if (typeof text != "string") {
                        throw new Error("Only string attachments are supported, received " + typeof attachment);
                    }
                    if (typeof filename != "string") {
                        throw new Error("Expected string type for filename but got " + typeof attachment);
                    }
                    if (!errorAttachments[report.id]) {
                        errorAttachments[report.id] = [];
                    }
                    errorAttachments[report.id].push({
                        text: text,
                        filename: filename
                    });
                }
                return Object.assign({
                    addTextAttachment
                }, report);
            });
            reports = reportsWithAttachmentFunction;
            callback(reports, function (response) {
                RNCrashes.crashUserResponse(response, errorAttachments);
            });
        }
    },

    setEventListener(listenerMap) {
        ReactNative.DeviceEventEmitter.removeAllListeners(willSendEvent);
        ReactNative.DeviceEventEmitter.removeAllListeners(sendDidSucceed);
        ReactNative.DeviceEventEmitter.removeAllListeners(sendDidFail);

        if (listenerMap && listenerMap.willSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(willSendEvent, listenerMap.willSendCrash);
        }
        if (listenerMap && listenerMap.didSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(sendDidSucceed, listenerMap.didSendCrash);
        }
        if (listenerMap && listenerMap.failedSendingCrash) {
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
