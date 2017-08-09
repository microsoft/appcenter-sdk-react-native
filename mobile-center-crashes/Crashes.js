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

    // async - returns a Promise
    process(callback) {

        // Checking enabled will make sure the callback is executed after the Android SDK has finished loading
        // crash reports in background. We could call getCrashReports too soon otherwise in case
        // it takes a lot of time.
        return RNCrashes.isEnabled()
            .then(enabled => {
                if (enabled) {
                    return RNCrashes.getCrashReports();
                }
            })
            .then(reports => {
                if (!reports) {
                    return;
                }
                let errorAttachments = {};
                let reportsWithAttachmentFunction = reports.map(function (report) {

                    // Add text attachment to an error report
                    function addTextAttachment(text, fileName) {
                        if (!errorAttachments[report.id]) {
                            errorAttachments[report.id] = [];
                        }
                        errorAttachments[report.id].push({
                            text: text,
                            fileName: fileName
                        });
                    };

                    // Add binary attachment to an error report, binary must be passed as a base64 string
                    function addBinaryAttachment(data, fileName, contentType) {
                        if (!errorAttachments[report.id]) {
                            errorAttachments[report.id] = [];
                        }
                        errorAttachments[report.id].push({
                            data: data,
                            fileName: fileName,
                            contentType: contentType
                        });
                    };

                    return Object.assign({
                        addTextAttachment,
                        addBinaryAttachment
                    }, report);
                });
                reports = reportsWithAttachmentFunction;
                callback(reports, function (response) {
                    RNCrashes.crashUserResponse(response, errorAttachments);
                });
            });
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
