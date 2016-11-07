package com.microsoft.azure.mobile.react.crashes;

import android.util.Log;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.sonoma.crashes.AbstractCrashesListener;
import com.microsoft.sonoma.crashes.ErrorAttachments;
import com.microsoft.sonoma.crashes.model.ErrorAttachment;
import com.microsoft.sonoma.crashes.model.ErrorReport;

import org.json.JSONException;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;


public abstract class RNCrashesListenerBase extends AbstractCrashesListener {
    private ReactApplicationContext mReactApplicationContext;
    private List<ErrorReport> mPendingReports = new ArrayList<ErrorReport>();

    protected ReadableMap mAttachments;

    private static final String ON_BEFORE_SENDING_EVENT = "MobileCenterErrorReportOnBeforeSending";
    private static final String ON_SENDING_FAILED_EVENT = "MobileCenterErrorReportOnSendingFailed";
    private static final String ON_SENDING_SUCCEEDED_EVENT = "MobileCenterErrorReportOnSendingSucceeded";

    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
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

    public final void provideAttachments(ReadableMap attachments) {
        this.mAttachments = attachments;
    }

    public final List<ErrorReport> getAndClearReports() {
        List<ErrorReport> reports = this.mPendingReports;
        this.mPendingReports = new ArrayList<ErrorReport>();;
        return reports;
    }

    protected final void storeReportForJS(ErrorReport report) {
        this.mPendingReports.add(report);
    }

    @Override
    public ErrorAttachment getErrorAttachment(ErrorReport report) {
        if (this.mAttachments == null) {
            return null;
        }

        try {
            String errorId = report.getId();
            if (this.mAttachments.hasKey(errorId)) {
                return ErrorAttachments.attachmentWithText(this.mAttachments.getString(errorId));
            }
        } catch (Exception e) {
            RNCrashesUtils.logError("Failed to get error attachment for report: " + report.getId());
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }

        return null;
    }

    @Override
    public final void onBeforeSending(ErrorReport report) {
        this.mAttachments = null;

        RNCrashesUtils.logInfo("Sending error report: " + report.getId());
        try {
            if(this.mReactApplicationContext != null) {
                this.mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_BEFORE_SENDING_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
            }
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onBeforeSending event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public final void onSendingFailed(ErrorReport report, Exception reason) {
        RNCrashesUtils.logError("Failed to send error report: " + report.getId());
        RNCrashesUtils.logError(Log.getStackTraceString(reason));
        try {
            if (this.mReactApplicationContext != null) {
                this.mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_SENDING_FAILED_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
            }
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onBeforeSending event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public final void onSendingSucceeded(ErrorReport report) {
        RNCrashesUtils.logInfo("Successfully Sent error report: " + report.getId());
        try {
            if (this.mReactApplicationContext != null) {
                this.mReactApplicationContext
                    .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                    .emit(ON_SENDING_SUCCEEDED_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
            }
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onSendingSucceeded event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }
}
