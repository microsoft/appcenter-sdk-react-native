package com.microsoft.azure.mobile.react.crashes;

import android.util.Log;

import com.facebook.react.bridge.LifecycleEventListener;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.azure.mobile.crashes.AbstractCrashesListener;
import com.microsoft.azure.mobile.crashes.model.ErrorReport;

import org.json.JSONException;

import java.util.AbstractMap;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

class RNCrashesListener extends AbstractCrashesListener {

    private static final String ON_BEFORE_SENDING_EVENT = "MobileCenterErrorReportOnBeforeSending";

    private static final String ON_SENDING_FAILED_EVENT = "MobileCenterErrorReportOnSendingFailed";

    private static final String ON_SENDING_SUCCEEDED_EVENT = "MobileCenterErrorReportOnSendingSucceeded";

    private ReactApplicationContext mReactApplicationContext;

    private List<Map.Entry<String, WritableMap>> mPendingEvents = new ArrayList<>();

    @SuppressWarnings("WeakerAccess")
    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public void onBeforeSending(ErrorReport report) {
        RNCrashesUtils.logInfo("Sending error report: " + report.getId());
        try {
            sendEvent(ON_BEFORE_SENDING_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onBeforeSending event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingFailed(ErrorReport report, Exception reason) {
        RNCrashesUtils.logError("Failed to send error report: " + report.getId());
        RNCrashesUtils.logError(Log.getStackTraceString(reason));
        try {
            sendEvent(ON_SENDING_FAILED_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onSendingFailed event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingSucceeded(ErrorReport report) {
        RNCrashesUtils.logInfo("Successfully Sent error report: " + report.getId());
        try {
            sendEvent(ON_SENDING_SUCCEEDED_EVENT, RNCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            RNCrashesUtils.logError("Failed to send onSendingSucceeded event:");
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    private void sendEvent(String eventType, WritableMap report) {
        if (this.mReactApplicationContext != null) {
            if (this.mReactApplicationContext.hasActiveCatalystInstance()) {
                this.mReactApplicationContext
                        .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                        .emit(eventType, report);
            } else {
                this.mPendingEvents.add(new AbstractMap.SimpleEntry<>(eventType, report));
                this.mReactApplicationContext.addLifecycleEventListener(lifecycleEventListener);
            }
        }
    }

    private void replayPendingEvents() {
        for (Map.Entry<String, WritableMap> event : this.mPendingEvents) {
            sendEvent(event.getKey(), event.getValue());
        }
        this.mPendingEvents.clear();
    }

    private LifecycleEventListener lifecycleEventListener = new LifecycleEventListener() {

        @Override
        public void onHostResume() {
            mReactApplicationContext.removeLifecycleEventListener(lifecycleEventListener);
            replayPendingEvents();
        }

        @Override
        public void onHostPause() {
        }

        @Override
        public void onHostDestroy() {
        }
    };
}
