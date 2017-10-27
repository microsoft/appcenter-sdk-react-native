const ReactNative = require('react-native');
const MobileCenterLog = require('mobile-center/mobile-center-log');

const { RNCrashes } = ReactNative.NativeModules;
const crashesEventEmitter = new ReactNative.NativeEventEmitter(RNCrashes);

const LOG_TAG = 'MobileCenterCrashes';
const EVENT_BEFORE_SENDING = 'MobileCenterErrorReportOnBeforeSending';
const EVENT_SENDING_SUCCEEDED = 'MobileCenterErrorReportOnSendingSucceeded';
const EVENT_SENDING_FAILED = 'MobileCenterErrorReportOnSendingFailed';

// This is set later if and when the user provides a value for the getErrorAttachments callback
let getErrorAttachmentsMethod = () => { };
const filteredReports = [];

const UserConfirmation = {
    DONT_SEND: 0,
    SEND: 1,
    ALWAYS_SEND: 2
};

const ErrorAttachmentLog = {

    // Create text attachment for an error report
    attachmentWithText(text, fileName) {
        return { text, fileName };
    },

    // Create binary attachment for an error report, binary must be passed as a base64 string
    attachmentWithBinary(data, fileName, contentType) {
        return { data, fileName, contentType };
    }
};

const Crashes = {

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

    notifyUserConfirmation(userConfirmation) {
        switch (userConfirmation) {
            case UserConfirmation.DONT_SEND:
            case UserConfirmation.SEND:
            case UserConfirmation.ALWAYS_SEND:
                RNCrashes.notifyWithUserConfirmation(userConfirmation);
                break;

            default:
                MobileCenterLog.error(LOG_TAG, 'Crashes.notifyUserConfirmation: Invalid parameter value.');
                return;
        }
        if (userConfirmation !== UserConfirmation.DONT_SEND) {
            Helper.sendErrorAttachments();
        }
    },

    setListener(listenerMap) {
        crashesEventEmitter.removeAllListeners(EVENT_BEFORE_SENDING);
        crashesEventEmitter.removeAllListeners(EVENT_SENDING_SUCCEEDED);
        crashesEventEmitter.removeAllListeners(EVENT_SENDING_FAILED);
        if (!listenerMap) {
            return;
        }
        if (listenerMap.onBeforeSending) {
            crashesEventEmitter.addListener(EVENT_BEFORE_SENDING, listenerMap.onBeforeSending);
        }
        if (listenerMap.onSendingSucceeded) {
            crashesEventEmitter.addListener(EVENT_SENDING_SUCCEEDED, listenerMap.onSendingSucceeded);
        }
        if (listenerMap.onSendingFailed) {
            crashesEventEmitter.addListener(EVENT_SENDING_FAILED, listenerMap.onSendingFailed);
        }
        getErrorAttachmentsMethod = listenerMap.getErrorAttachments;
        RNCrashes.getUnprocessedCrashReports()
            .then((reports) => {
                if (reports.length > 0) {
                    const filteredReportIds = [];
                    reports.forEach((report) => {
                        if (!listenerMap.shouldProcess ||
                            listenerMap.shouldProcess(report)) {
                            filteredReports.push(report);
                            filteredReportIds.push(report.id);
                        }
                    });
                    RNCrashes.sendCrashReportsOrAwaitUserConfirmationForFilteredIds(filteredReportIds).then((alwaysSend) => {
                        if (alwaysSend) {
                            Helper.sendErrorAttachments();
                        } else if (!listenerMap.shouldAwaitUserConfirmation || !listenerMap.shouldAwaitUserConfirmation()) {
                            Crashes.notifyUserConfirmation(UserConfirmation.SEND);
                        }
                    });
                }
            });
    }
};

const Helper = {
    sendErrorAttachments() {
        if (!getErrorAttachmentsMethod) {
            return;
        }
        filteredReports.forEach((report) => {
            const attachments = getErrorAttachmentsMethod(report);
            RNCrashes.sendErrorAttachments(attachments, report.id);
        });

        // Prevent multipe calls if shouldAwaitUserConfirmation is false and user calling notifyUserConfirmation for some reason
        filteredReports.length = 0;
    }
};

// Exports with "curly braces".
Crashes.UserConfirmation = UserConfirmation;
Crashes.ErrorAttachmentLog = ErrorAttachmentLog;

// Export main class without "curly braces".
module.exports = Crashes;
