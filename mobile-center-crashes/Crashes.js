const ReactNative = require('react-native');
const MobileCenterLog = require('mobile-center/mobile-center-log');

const logTag = 'MobileCenter';
const RNCrashes = ReactNative.NativeModules.RNCrashes;
const RNWrapperCrashesHelper = ReactNative.NativeModules.RNWrapperCrashesHelper;

const willSendEvent = 'MobileCenterErrorReportOnBeforeSending';
const sendDidSucceed = 'MobileCenterErrorReportOnSendingSucceeded';
const sendDidFail = 'MobileCenterErrorReportOnSendingFailed';

reports = new Array(0);

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
        //TODO implement
    },
    // async - returns a Promise
    process(callback) {
        // Calling .isEnabled() will make sure the callback is executed after the Android SDK has finished loading
        // crash reports in background. We could call getCrashReports too soon otherwise in case
        // it takes a lot of time.
        return RNCrashes.isEnabled()
            .then((enabled) => {
                if (enabled) {
                    return RNCrashes.getCrashReports();
                }
                MobileCenterLog.error(logTag, 'Could not get crash reports when Mobile Center crashes is not enabled.');
                return Promise.reject('Mobile Center crashes is not enabled.');
            })
            .then((reports) => {
                if (!reports) {
                    return;
                }
                const errorAttachments = {};
                const reportsWithAttachmentFunction = reports.map((report) => {
                    // Add text attachment to an error report
                    function addTextAttachment(text, fileName) {
                        if (!errorAttachments[report.id]) {
                            errorAttachments[report.id] = [];
                        }
                        errorAttachments[report.id].push({
                            text,
                            fileName
                        });
                    }

                    // Add binary attachment to an error report, binary must be passed as a base64 string
                    function addBinaryAttachment(data, fileName, contentType) {
                        if (!errorAttachments[report.id]) {
                            errorAttachments[report.id] = [];
                        }
                        errorAttachments[report.id].push({
                            data,
                            fileName,
                            contentType
                        });
                    }

                    return Object.assign({
                        addTextAttachment,
                        addBinaryAttachment
                    }, report);
                });
                reports = reportsWithAttachmentFunction;
                callback(reports, (response) => {
                    RNCrashes.crashUserResponse(response, errorAttachments);
                });
            });
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
        .then((reports) =>
        {
            filteredReports = new Array();
            errorAttachments = {};
            reports.forEach((report) => {
                if (!listenerMap.shouldProcess ||
                    listenerMap.shouldProcess(report)) {
                    filteredReports.push(report);
                    if (listenerMap.getErrorAttachments) {
                        attachments = listenerMap.getErrorAttachments(report);
                        //TODO need to convert attachments?
                        //TODO check necessary?
                        if (attachments && attachments.length > 0) {
                            // Add the error attachments
                            errorAttachments[report["id"]] = attachments;
                        }
                    }
                }
            });

            //TODO need to send error attachments in the right place.
            RNWrapperCrashesHelper.sendCrashReportsOrAwaitUserConfirmationForFilteredList(filteredReports);
            
            if (listnerMap.shouldAwaitUserConfirmation &&
                listenerMap.shouldAwaitUserConfirmation()) {
                    // TODO don't send attachments here, but make sure they get sent when notified
                }
            else {
                //TODO notifyWithUserConfirmation(send);
                // TODO don't send attachments here, but make sure they get sent when notified.
                }
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
