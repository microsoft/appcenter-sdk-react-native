// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.crashes;

import android.app.Application;
import android.util.Base64;
import android.util.Log;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.ReadableMapKeySetIterator;
import com.facebook.react.bridge.ReadableType;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.crashes.Crashes;
import com.microsoft.appcenter.crashes.WrapperSdkExceptionManager;
import com.microsoft.appcenter.crashes.ingestion.models.ErrorAttachmentLog;
import com.microsoft.appcenter.crashes.ingestion.models.Exception;
import com.microsoft.appcenter.crashes.ingestion.models.StackFrame;
import com.microsoft.appcenter.crashes.model.ErrorReport;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import org.json.JSONException;

import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativeCrashesModule extends BaseJavaModule {

    private static final String DATA_FIELD = "data";
    private static final String TEXT_FIELD = "text";
    private static final String FILE_NAME_FIELD = "fileName";
    private static final String CONTENT_TYPE_FIELD = "contentType";

    /**
     * Constant for DO NOT SEND crash report.
     */
    private static final int DONT_SEND = 0;

    /**
     * Constant for SEND crash report.
     */
    private static final int SEND = 1;

    /**
     * Constant for ALWAYS SEND crash reports.
     */
    private static final int ALWAYS_SEND = 2;

    private AppCenterReactNativeCrashesListener mCrashListener;

    public AppCenterReactNativeCrashesModule(Application application, boolean automaticProcessing) {
        mCrashListener = new AppCenterReactNativeCrashesListener();
        WrapperSdkExceptionManager.setAutomaticProcessing(automaticProcessing);
        Crashes.setListener(mCrashListener);
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            AppCenter.start(Crashes.class);
        }
    }

    public void setReactApplicationContext(ReactApplicationContext reactContext) {
        AppCenterReactNativeCrashesUtils.logDebug("Setting react context");
        this.mCrashListener.setReactApplicationContext(reactContext);
    }

    @Override
    public String getName() {
        return "AppCenterReactNativeCrashes";
    }

    @Override
    public Map<String, Object> getConstants() {
        return new HashMap<>();
    }

    @ReactMethod
    public void lastSessionCrashReport(final Promise promise) {
        Crashes.getLastSessionCrashReport().thenAccept(new AppCenterConsumer<ErrorReport>() {

            @Override
            public void accept(ErrorReport errorReport) {
                promise.resolve(errorReport != null ?
                        AppCenterReactNativeCrashesUtils.convertErrorReportToWritableMapOrEmpty(errorReport)
                        : null);
            }
        });
    }

    @ReactMethod
    public void hasCrashedInLastSession(final Promise promise) {
        Crashes.hasCrashedInLastSession().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean hasCrashed) {
                promise.resolve(hasCrashed);
            }
        });
    }

    @ReactMethod
    public void hasReceivedMemoryWarningInLastSession(final Promise promise) {
        Crashes.hasReceivedMemoryWarningInLastSession().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean hasWarning) {
                promise.resolve(hasWarning);
            }
        });
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Crashes.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Crashes.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    public void trackError(ReadableMap error, ReadableMap properties, ReadableArray attachments, final Promise promise) {

        Exception exceptionModel;
        Map<String, String> convertedProperties;
        try {
            exceptionModel = toExceptionModel(error);
            convertedProperties = convertReadableMapToStringMap(properties);
        } catch (java.lang.Exception ex) {
            return;
        }

        String errorId = WrapperSdkExceptionManager.trackException(exceptionModel, convertedProperties, toCustomErrorAttachments(attachments));
        promise.resolve(errorId);
    }

    @ReactMethod
    public void generateTestCrash(final Promise promise) {
        new Thread(new Runnable() {

            @Override
            public void run() {
                Crashes.generateTestCrash();
                promise.resolve(null);
            }
        }).start();
    }

    @ReactMethod
    public void notifyWithUserConfirmation(int userConfirmation) {

        /* Translate JS constant to Android. Android uses different order of enum than JS/iOS/.NET. */
        switch (userConfirmation) {

            case DONT_SEND:
                userConfirmation = Crashes.DONT_SEND;
                break;

            case SEND:
                userConfirmation = Crashes.SEND;
                break;

            case ALWAYS_SEND:
                userConfirmation = Crashes.ALWAYS_SEND;
                break;
        }

        /* Pass translated value, if not translated, native SDK should check the value itself for error. */
        Crashes.notifyUserConfirmation(userConfirmation);
    }

    @ReactMethod
    public void getUnprocessedCrashReports(final Promise promise) {
        WrapperSdkExceptionManager.getUnprocessedErrorReports().thenAccept(new AppCenterConsumer<Collection<ErrorReport>>() {

            @Override
            public void accept(Collection<ErrorReport> errorReports) {
                promise.resolve(AppCenterReactNativeCrashesUtils.convertErrorReportsToWritableArrayOrEmpty(errorReports));
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
        WrapperSdkExceptionManager.sendCrashReportsOrAwaitUserConfirmation(filteredReportIdsAsList).thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean alwaysSend) {
                promise.resolve(alwaysSend);
            }
        });
    }

    @ReactMethod
    public void sendErrorAttachments(ReadableArray attachments, String errorId) {
        WrapperSdkExceptionManager.sendErrorAttachments(errorId, toCustomErrorAttachments(attachments));
    }

    static Collection<ErrorAttachmentLog> toCustomErrorAttachments(ReadableArray attachments) {
        Collection<ErrorAttachmentLog> attachmentLogs = new LinkedList<>();
        try {
            for (int i = 0; i < attachments.size(); i++) {
                ReadableMap jsAttachment = attachments.getMap(i);
                String fileName = null;
                if (jsAttachment.hasKey(FILE_NAME_FIELD)) {
                    fileName = jsAttachment.getString(FILE_NAME_FIELD);
                }
                if (jsAttachment.hasKey(TEXT_FIELD)) {
                    String text = jsAttachment.getString(TEXT_FIELD);
                    attachmentLogs.add(ErrorAttachmentLog.attachmentWithText(text, fileName));
                } else {
                    String encodedData = jsAttachment.getString(DATA_FIELD);
                    byte[] data = Base64.decode(encodedData, Base64.DEFAULT);
                    String contentType = jsAttachment.getString(CONTENT_TYPE_FIELD);
                    attachmentLogs.add(ErrorAttachmentLog.attachmentWithBinary(data, fileName, contentType));
                }
            }
        } catch (java.lang.Exception e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to get error attachment for report: " + attachments);
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
        return attachmentLogs;
    }

       static Exception toExceptionModel(ReadableMap readableMap)
               throws java.lang.Exception {
        Exception model = new Exception();
        try {
            String type = readableMap.getString("type");
            String message = readableMap.getString("message");
            if (type == null || type == "") {
                throw new java.lang.Exception("Type value shouldn't be null ot empty");
            }
            if (message == null || message == "") {
                throw new java.lang.Exception("Message value shouldn't be null or empty");
            }
            model.setType(type);
            model.setMessage(message);
            model.setStackTrace(readableMap.getString("stackTrace"));
            List<StackFrame> msFrames = new ArrayList<StackFrame>();
            ReadableArray frames = readableMap.getArray("frames");
            for (int i = 0; i < frames.size(); ++i)
            {
                ReadableMap frame = frames.getMap(i);
                StackFrame msFrame = new StackFrame();
                msFrame.setFileName(frame.getString("fileName"));


                msFrame.setLineNumber(frame.getInt("lineNumber"));
                msFrame.setMethodName(frame.getString("methodName"));
                msFrame.setClassName(frame.getString("className"));
//                msFrame.setCode(frame.getString("code"));
//                msFrame.setAddress(frame.getString("address"));
                msFrames.add(msFrame);
            }
            model.setFrames(msFrames);
        } catch (java.lang.Exception e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to get exception model");
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
        return model;
    }

    static Map<String, String> convertReadableMapToStringMap(ReadableMap map) throws JSONException {
        Map<String, String> stringMap = new HashMap<>();
        ReadableMapKeySetIterator it = map.keySetIterator();
        while (it.hasNextKey()) {
            String key = it.nextKey();
            ReadableType type = map.getType(key);
            // Only support storing strings. Non-string data must be stringified in JS.
            if (type == ReadableType.String) {
                stringMap.put(key, map.getString(key));
            }
        }

        return stringMap;
    }

}
