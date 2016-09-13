package com.microsoft.react.sonoma.errorreporting;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.sonoma.crashes.Crashes;
import com.microsoft.sonoma.crashes.model.ErrorReport;

import org.json.JSONException;

import java.io.IOException;

public class RNSonomaErrorReportingModule extends ReactContextBaseJavaModule {

    public RNSonomaErrorReportingModule(ReactApplicationContext reactContext) {
        super(reactContext);
        Crashes.setListener(new RNSonomaCrashesListener(reactContext));
    }

    @Override
    public String getName() {
        return "RNSonomaErrorReporting";
    }

    @ReactMethod
    public void generateTestCrash(final Promise promise) {
        new Thread(new Runnable() {
            public void run() {
                Crashes.generateTestCrash();
                promise.resolve("");
            }
        }).start();
    }

    @ReactMethod
    public void hasCrashedInLastSession(Promise promise) {
        promise.resolve(Crashes.hasCrashedInLastSession());
    }

    @ReactMethod
    public void sendCrashes(Promise promise) {
        Crashes.notifyUserConfirmation(Crashes.SEND);
        promise.resolve("");
    }

    @ReactMethod
    public void ignoreCrashes(Promise promise) {
        Crashes.notifyUserConfirmation(Crashes.DONT_SEND);
        promise.resolve("");
    }

    @ReactMethod
    public void setTextAttachment(String textAttachment, Promise promise) {
        ErrorReport lastSessionCrashReport = Crashes.getLastSessionCrashReport();
        try {
            RNSonomaErrorAttachmentHelper.saveTextAttachment(getReactApplicationContext(), lastSessionCrashReport, textAttachment);
            promise.resolve("");
        } catch (IOException e) {
            promise.reject(e);
        }
    }

    @ReactMethod
    public void getLastSessionCrashDetails(Promise promise) {
        ErrorReport lastSessionCrashReport = Crashes.getLastSessionCrashReport();
        try {
            promise.resolve(RNSonomaErrorReportingUtils.convertErrorReportToWritableMap(lastSessionCrashReport));
        } catch (JSONException e) {
            promise.reject(e);
        }
    }
}
