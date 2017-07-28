package com.microsoft.azure.mobile.react.crashes;

import android.util.Base64;
import android.util.Log;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.azure.mobile.crashes.AbstractCrashesListener;
import com.microsoft.azure.mobile.crashes.ingestion.models.ErrorAttachmentLog;
import com.microsoft.azure.mobile.crashes.model.ErrorReport;

import org.json.JSONException;

import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedList;
import java.util.List;

abstract class RNCrashesListenerBase extends AbstractCrashesListener {

    private static final String ON_BEFORE_SENDING_EVENT = "MobileCenterErrorReportOnBeforeSending";

    private static final String ON_SENDING_FAILED_EVENT = "MobileCenterErrorReportOnSendingFailed";

    private static final String ON_SENDING_SUCCEEDED_EVENT = "MobileCenterErrorReportOnSendingSucceeded";

    private ReactApplicationContext mReactApplicationContext;

    private List<ErrorReport> mPendingReports = new ArrayList<>();

    private ReadableMap mAttachments;

    @SuppressWarnings("WeakerAccess")
    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    void setAttachments(ReadableMap mAttachments) {
        this.mAttachments = mAttachments;
    }

    @SuppressWarnings("WeakerAccess")
    public final List<ErrorReport> getAndClearReports() {
        List<ErrorReport> reports = this.mPendingReports;
        this.mPendingReports = new ArrayList<>();
        return reports;
    }

    @SuppressWarnings("WeakerAccess")
    protected final void storeReportForJS(ErrorReport report) {
        this.mPendingReports.add(report);
    }

    @Override
    public Iterable<ErrorAttachmentLog> getErrorAttachments(ErrorReport report) {
        if (this.mAttachments != null) {
            Collection<ErrorAttachmentLog> attachmentLogs = new LinkedList<>();
            try {
                String errorId = report.getId();
                ReadableArray jsAttachmentsForReport = mAttachments.getArray(errorId);
                if (jsAttachmentsForReport != null) {
                    for (int i = 0; i < jsAttachmentsForReport.size(); i++) {
                        ReadableMap jsAttachment = jsAttachmentsForReport.getMap(i);
                        String fileName = jsAttachment.getString("fileName");
                        if (jsAttachment.hasKey("text")) {
                            String text = jsAttachment.getString("text");
                            attachmentLogs.add(ErrorAttachmentLog.attachmentWithText(text, fileName));
                        } else {
                            String encodedData = jsAttachment.getString("data");
                            byte[] data = Base64.decode(encodedData, Base64.DEFAULT);
                            String contentType = jsAttachment.getString("contentType");
                            attachmentLogs.add(ErrorAttachmentLog.attachmentWithBinary(data, fileName, contentType));
                        }
                    }
                }
                return attachmentLogs;
            } catch (Exception e) {
                RNCrashesUtils.logError("Failed to get error attachment for report: " + report.getId());
                RNCrashesUtils.logError(Log.getStackTraceString(e));
            }
        }
        return null;
    }

    @Override
    public final void onBeforeSending(ErrorReport report) {
        RNCrashesUtils.logInfo("Sending error report: " + report.getId());
        try {
            if (this.mReactApplicationContext != null) {
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
