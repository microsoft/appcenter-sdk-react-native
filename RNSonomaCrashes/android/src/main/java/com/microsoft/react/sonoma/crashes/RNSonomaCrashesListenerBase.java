package com.microsoft.react.sonoma.crashes;

import android.util.Log;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.sonoma.crashes.AbstractCrashesListener;
import com.microsoft.sonoma.crashes.ErrorAttachments;
import com.microsoft.sonoma.crashes.model.ErrorAttachment;
import com.microsoft.sonoma.crashes.model.ErrorReport;

import org.json.JSONException;

import java.io.IOException;

public abstract class RNSonomaCrashesListenerBase extends AbstractCrashesListener {
    private ReactApplicationContext mReactApplicationContext;
    private static final String ON_BEFORE_SENDING_EVENT = "SonamaErrorReportOnBeforeSending";
    private static final String ON_SENDING_FAILED_EVENT = "SonamaErrorReportOnSendingFailed";
    private static final String ON_SENDING_SUCCEEDED_EVENT = "SonamaErrorReportOnSendingSucceeded";

    public RNSonomaCrashesListener() {
    }

    public void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    /**
     * Called when the JavaScript responds to a crash. 
     * May be used when deciding whether to delay sending a crash,
     * e.g. to determine that the JavaScript environment is unable to respond
     * and thus to make decision in native code.
     *
     * @param userConfirmation either Crashes.SEND or Crashes.DONT_SEND
     */
    public void reportUserResponse(int userConfirmation) { }

    @Override
    public ErrorAttachment getErrorAttachment(ErrorReport report) {
        try {
            String textAttachment = RNSonomaErrorAttachmentHelper.getTextAttachment(mReactApplicationContext, report);
            if (textAttachment != null) {
                return ErrorAttachments.attachmentWithText(textAttachment);
            }
        } catch (IOException e) {
            RNSonomaCrashesUtils.logError("Failed to get error attachment for report: " + report.getId());
            RNSonomaCrashesUtils.logError(Log.getStackTraceString(e));
        }

        return null;
    }

    @Override
    public void onBeforeSending(ErrorReport report) {
        RNSonomaCrashesUtils.logInfo("Sending error report: " + report.getId());
        try {
            mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_BEFORE_SENDING_EVENT, RNSonomaCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNSonomaCrashesUtils.logError("Failed to send onBeforeSending event:");
            RNSonomaCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingFailed(ErrorReport report, Exception reason) {
        RNSonomaCrashesUtils.logError("Failed to send error report: " + report.getId());
        RNSonomaCrashesUtils.logError(Log.getStackTraceString(reason));
        try {
            mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_SENDING_FAILED_EVENT, RNSonomaCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNSonomaCrashesUtils.logError("Failed to send onBeforeSending event:");
            RNSonomaCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingSucceeded(ErrorReport report) {
        RNSonomaCrashesUtils.logInfo("Successfully Sent error report: " + report.getId());
        try {
            mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_SENDING_SUCCEEDED_EVENT, RNSonomaCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNSonomaCrashesUtils.logError("Failed to send onSendingSucceeded event:");
            RNSonomaCrashesUtils.logError(Log.getStackTraceString(e));
        }

        RNSonomaErrorAttachmentHelper.deleteTextAttachment(mReactApplicationContext, report);
    }
}
