package com.microsoft.azure.mobile.react.crashes;

import android.app.Application;
import android.util.Log;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.WritableNativeMap;

import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.crashes.Crashes;
import com.microsoft.azure.mobile.crashes.model.ErrorReport;

import com.microsoft.azure.mobile.react.mobilecenter.RNMobileCenter;
import com.microsoft.azure.mobile.ResultCallback;

import org.json.JSONException;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class RNCrashesModule extends BaseJavaModule {
    private RNCrashesListenerBase mCrashListener;

    private static final String HasCrashedInLastSessionKey = "hasCrashedInLastSession";
    private static final String LastCrashReportKey = "lastCrashReport";

    public RNCrashesModule(Application application, RNCrashesListenerBase crashListener) {
        this.mCrashListener = crashListener;
        if (crashListener != null) {
            Crashes.setListener(crashListener);
        }

        RNMobileCenter.configureMobileCenter(application);
        MobileCenter.start(Crashes.class);
    }

    public void setReactApplicationContext(ReactApplicationContext reactContext) {
        RNCrashesUtils.logDebug("Setting react context");
        if (this.mCrashListener != null) {
            this.mCrashListener.setReactApplicationContext(reactContext);
        }
    }

    @Override
    public String getName() {
        return "RNCrashes";
    }

    @Override
    public Map<String, Object> getConstants() {
        final Map<String, Object> constants = new HashMap<>();
        return constants;
    }

    @ReactMethod
    public void lastSessionCrashReport(final Promise promise) {
        Crashes.getLastSessionCrashReport(new ResultCallback<ErrorReport>() {
            @Override
            public void onResult(ErrorReport errorReport) {
                promise.resolve(errorReport != null ? 
                    RNCrashesUtils.convertErrorReportToWritableMapOrEmpty(errorReport)
                    : null);
            }
        });
    }

    @ReactMethod
    public void hasCrashedInLastSession(final Promise promise) {
        Crashes.getLastSessionCrashReport(new ResultCallback<ErrorReport>() {
            @Override
            public void onResult(ErrorReport errorReport) {
                Boolean hasCrashed = errorReport != null;
                promise.resolve(hasCrashed);
            }
        });
    }

    @ReactMethod
    public void getCrashReports(Promise promise) {
        List<ErrorReport> pendingReports = this.mCrashListener.getAndClearReports();
        promise.resolve(RNCrashesUtils.convertErrorReportsToWritableArrayOrEmpty(pendingReports));
    }

    @ReactMethod
    public void setEnabled(boolean shouldEnable) {
        Crashes.setEnabled(shouldEnable);
    }

    @ReactMethod
    public void isEnabled(Promise promise) {
        promise.resolve(Crashes.isEnabled());
    }

    @ReactMethod
    public void generateTestCrash(final Promise promise) {
        new Thread(new Runnable() {
            public void run() {
                Crashes.generateTestCrash();
                promise.reject(new Exception("generateTestCrash failed to generate a crash"));
            }
        }).start();
    }

    @ReactMethod 
    public void crashUserResponse(boolean send, ReadableMap attachments, Promise promise) {
        int response = send ? Crashes.SEND : Crashes.DONT_SEND;
        if (mCrashListener != null) {
            mCrashListener.reportUserResponse(response);
            //TODO: Re-enable error attachment when the feature becomes available.
            //mCrashListener.provideAttachments(attachments);
        }
        Crashes.notifyUserConfirmation(response);
        promise.resolve("");
    }
}
