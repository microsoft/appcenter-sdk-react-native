// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.crashes;

import android.util.Log;

import com.facebook.react.bridge.LifecycleEventListener;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.appcenter.crashes.AbstractCrashesListener;
import com.microsoft.appcenter.crashes.model.ErrorReport;

import org.json.JSONException;

import java.util.AbstractMap;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

class AppCenterReactNativeCrashesListener extends AbstractCrashesListener {

    private static final String ON_BEFORE_SENDING_EVENT = "AppCenterErrorReportOnBeforeSending";

    private static final String ON_SENDING_FAILED_EVENT = "AppCenterErrorReportOnSendingFailed";

    private static final String ON_SENDING_SUCCEEDED_EVENT = "AppCenterErrorReportOnSendingSucceeded";

    private ReactApplicationContext mReactApplicationContext;

    private List<Map.Entry<String, WritableMap>> mPendingEvents = new ArrayList<>();

    @SuppressWarnings("WeakerAccess")
    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public void onBeforeSending(ErrorReport report) {
        AppCenterReactNativeCrashesUtils.logInfo("Sending error report: " + report.getId());
        try {
            sendEvent(ON_BEFORE_SENDING_EVENT, AppCenterReactNativeCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to send onBeforeSending event:");
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingFailed(ErrorReport report, Exception reason) {
        AppCenterReactNativeCrashesUtils.logError("Failed to send error report: " + report.getId());
        AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(reason));
        try {
            sendEvent(ON_SENDING_FAILED_EVENT, AppCenterReactNativeCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to send onSendingFailed event:");
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }

    @Override
    public void onSendingSucceeded(ErrorReport report) {
        AppCenterReactNativeCrashesUtils.logInfo("Successfully Sent error report: " + report.getId());
        try {
            sendEvent(ON_SENDING_SUCCEEDED_EVENT, AppCenterReactNativeCrashesUtils.convertErrorReportToWritableMap(report));
        } catch (JSONException e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to send onSendingSucceeded event:");
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
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
