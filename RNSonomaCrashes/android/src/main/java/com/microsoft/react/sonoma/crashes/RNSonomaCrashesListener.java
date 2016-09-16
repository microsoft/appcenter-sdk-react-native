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

public class RNSonomaCrashesListener extends AbstractCrashesListener{
    private ReactApplicationContext mReactApplicationContext;
    private static final String ON_BEFORE_SENDING_EVENT = "SonamaErrorReportOnBeforeSending";
    private static final String ON_SENDING_FAILED_EVENT = "SonamaErrorReportOnSendingFailed";
    private static final String ON_SENDING_SUCCEEDED_EVENT = "SonamaErrorReportOnSendingSucceeded";

    public RNSonomaCrashesListener(ReactApplicationContext reactApplicationContext) {
        mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public boolean shouldProcess(ErrorReport report) {
        // Process all crashes by default. JS side can stop a crash from
        // being reported via the user confirmation "DONT_SEND" signal.
        return true;
    }

    @Override
    public boolean shouldAwaitUserConfirmation() {
        // Require user confirmation for all crashes, since this is the
        // only way JS can indicate whether or not a crash should be sent.
        return true;
    }

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
