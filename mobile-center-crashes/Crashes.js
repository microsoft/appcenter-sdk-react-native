const ReactNative = require('react-native');
const MobileCenterLog = require('mobile-center/mobile-center-log');

const logTag = 'MobileCenter';
const RNCrashes = ReactNative.NativeModules.RNCrashes;
const RNWrapperCrashesHelper = ReactNative.NativeModules.RNWrapperCrashesHelper;

const willSendEvent = 'MobileCenterErrorReportOnBeforeSending';
const sendDidSucceed = 'MobileCenterErrorReportOnSendingSucceeded';
const sendDidFail = 'MobileCenterErrorReportOnSendingFailed';

let UserConfirmation = {
    Send : 1,
    DontSend : 2,
    AlwaySend : 3
}

let ErrorAttachmentLog = {
    // Create text attachment for an error report
    attachmentWithText(text, fileName) {
        return { text, fileName };
    },

    // Create binary attachment for an error report, binary must be passed as a base64 string
    attachmentWithBinary(data, fileName, contentType) {
        return { data, fileName, contentType };
    }
};

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
    notifyWithUserConfirmation(userConfirmation) {
        RNCrashes.notifyWithUserConfirmation(userConfirmation);
    },

    setEventListener(listenerMap) {
        ReactNative.DeviceEventEmitter.removeAllListeners(willSendEvent);
        ReactNative.DeviceEventEmitter.removeAllListeners(sendDidSucceed);
        ReactNative.DeviceEventEmitter.removeAllListeners(sendDidFail);   
        if (!listenerMap) {
            return;
        }
        if (listenerMap.willSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(willSendEvent, listenerMap.willSendCrash);
        }
        if (listenerMap.didSendCrash) {
            ReactNative.DeviceEventEmitter.addListener(sendDidSucceed, listenerMap.didSendCrash);
        }
        if (listenerMap.failedSendingCrash) {
            ReactNative.DeviceEventEmitter.addListener(sendDidFail, listenerMap.failedSendingCrash);
        }
        RNWrapperCrashesHelper.getUnprocessedCrashReports()
        .then((reports) => {
            filteredReports = [];
            reports.forEach((report) => {
                if (!listenerMap.shouldProcess ||
                    listenerMap.shouldProcess(report)) {
                    filteredReports.push(report);
                    if (listenerMap.getErrorAttachments) {
                        errorAttachments = listenerMap.getErrorAttachments(report);

                        // Send error attachments for the report.
                        RNWrapperCrashesHelper.sendErrorAttachments(errorAttachments, report["id"]);
                    }
                }
            });
            RNWrapperCrashesHelper.sendCrashReportsOrAwaitUserConfirmationForFilteredList(filteredReports);
        });
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
