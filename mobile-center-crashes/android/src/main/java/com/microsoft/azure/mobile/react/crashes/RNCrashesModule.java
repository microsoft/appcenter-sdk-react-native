package com.microsoft.azure.mobile.react.crashes;

import android.app.Application;
import android.util.Base64;
import android.util.Log;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.crashes.Crashes;
import com.microsoft.azure.mobile.crashes.WrapperSdkExceptionManager;
import com.microsoft.azure.mobile.crashes.ingestion.models.ErrorAttachmentLog;
import com.microsoft.azure.mobile.crashes.model.ErrorReport;
import com.microsoft.azure.mobile.react.mobilecentershared.RNMobileCenterShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;

@SuppressWarnings("WeakerAccess")
public class RNCrashesModule extends BaseJavaModule {

    private RNCrashesListener mCrashListener;

    public RNCrashesModule(Application application, boolean automaticProcessing) {
        mCrashListener = new RNCrashesListener();
        WrapperSdkExceptionManager.setAutomaticProcessing(automaticProcessing);
        Crashes.setListener(mCrashListener);
        RNMobileCenterShared.configureMobileCenter(application);
        MobileCenter.start(Crashes.class);
    }

    public void setReactApplicationContext(ReactApplicationContext reactContext) {
        RNCrashesUtils.logDebug("Setting react context");
        this.mCrashListener.setReactApplicationContext(reactContext);
    }

    @Override
    public String getName() {
        return "RNCrashes";
    }

    @Override
    public Map<String, Object> getConstants() {
        return new HashMap<>();
    }

    @ReactMethod
    public void lastSessionCrashReport(final Promise promise) {
        Crashes.getLastSessionCrashReport().thenAccept(new MobileCenterConsumer<ErrorReport>() {

            @Override
            public void accept(ErrorReport errorReport) {
                promise.resolve(errorReport != null ?
                        RNCrashesUtils.convertErrorReportToWritableMapOrEmpty(errorReport)
                        : null);
            }
        });
    }

    @ReactMethod
    public void hasCrashedInLastSession(final Promise promise) {
        Crashes.hasCrashedInLastSession().thenAccept(new MobileCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean hasCrashed) {
                promise.resolve(hasCrashed);
            }
        });
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Crashes.setEnabled(enabled).thenAccept(new MobileCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Crashes.isEnabled().thenAccept(new MobileCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    @ReactMethod
    public void generateTestCrash(final Promise promise) {
        new Thread(new Runnable() {

            @Override
            public void run() {
                Crashes.generateTestCrash();
                promise.reject(new Exception("generateTestCrash failed to generate a crash"));
            }
        }).start();
    }

    @ReactMethod
    public void notifyWithUserConfirmation(int userConfirmation) {
        Crashes.notifyUserConfirmation(userConfirmation - 1);
    }

    @ReactMethod
    public void getUnprocessedCrashReports(final Promise promise) {
        WrapperSdkExceptionManager.getUnprocessedErrorReports().thenAccept(new MobileCenterConsumer<Collection<ErrorReport>>() {

            @Override
            public void accept(Collection<ErrorReport> errorReports) {
                promise.resolve(RNCrashesUtils.convertErrorReportsToWritableArrayOrEmpty(errorReports));
            }
        });
    }

    @ReactMethod
    public void sendCrashReportsOrAwaitUserConfirmationForFilteredIds(ReadableArray filteredReportIds, final Promise promise) {
        int size = filteredReportIds.size();
        Collection<String> filteredReportIdsAsList = new ArrayList<>(size);
        for (int i = 0; i < size; i++) {
            filteredReportIdsAsList.add(filteredReportIds.getString(i));
        }
        WrapperSdkExceptionManager.sendCrashReportsOrAwaitUserConfirmation(filteredReportIdsAsList).thenAccept(new MobileCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean alwaysSend) {
                promise.resolve(alwaysSend);
            }
        });
    }

    @ReactMethod
    public void sendErrorAttachments(ReadableArray attachments, String errorId) {
        try {
            Collection<ErrorAttachmentLog> attachmentLogs = new LinkedList<>();
            for (int i = 0; i < attachments.size(); i++) {
                ReadableMap jsAttachment = attachments.getMap(i);
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
            WrapperSdkExceptionManager.sendErrorAttachments(errorId, attachmentLogs);
        } catch (Exception e) {
            RNCrashesUtils.logError("Failed to get error attachment for report: " + errorId);
            RNCrashesUtils.logError(Log.getStackTraceString(e));
        }
    }
}
