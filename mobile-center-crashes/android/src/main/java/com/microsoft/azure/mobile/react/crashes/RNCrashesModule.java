package com.microsoft.azure.mobile.react.crashes;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.crashes.Crashes;
import com.microsoft.azure.mobile.crashes.model.ErrorReport;
import com.microsoft.azure.mobile.react.mobilecentershared.RNMobileCenterShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

@SuppressWarnings("WeakerAccess")
public class RNCrashesModule extends BaseJavaModule {

    private RNCrashesListenerBase mCrashListener;

    public RNCrashesModule(Application application, RNCrashesListenerBase crashListener) {
        this.mCrashListener = crashListener;
        if (crashListener != null) {
            Crashes.setListener(crashListener);
        }

        RNMobileCenterShared.configureMobileCenter(application);
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
    public void getCrashReports(Promise promise) {
        List<ErrorReport> pendingReports = this.mCrashListener.getAndClearReports();
        promise.resolve(RNCrashesUtils.convertErrorReportsToWritableArrayOrEmpty(pendingReports));
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
    public void crashUserResponse(boolean send, ReadableMap attachments, Promise promise) {
        int response = send ? Crashes.SEND : Crashes.DONT_SEND;
        if (mCrashListener != null) {
            mCrashListener.setAttachments(attachments);
        }
        Crashes.notifyUserConfirmation(response);
        promise.resolve("");
    }
}
