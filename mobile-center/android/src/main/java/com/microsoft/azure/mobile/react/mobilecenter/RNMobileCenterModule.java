package com.microsoft.azure.mobile.react.mobilecenter;

import android.app.Application;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;

import com.microsoft.azure.mobile.react.mobilecentershared.RNMobileCenterShared;
import com.microsoft.azure.mobile.MobileCenter;

import org.json.JSONException;

public class RNMobileCenterModule extends BaseJavaModule {

    public RNMobileCenterModule(Application application) {
        RNMobileCenterShared.configureMobileCenter(application);
    }

    @Override
    public String getName() {
        return "RNMobileCenter";
    }

    @ReactMethod
    public void setEnabled(boolean enabled) {
        MobileCenter.setEnabled(enabled);
    }

    @ReactMethod
    public void setLogLevel(int logLevel) {
        MobileCenter.setLogLevel(logLevel);
    }

    @ReactMethod
    public void getLogLevel(final Promise promise) {
        int logLevel = MobileCenter.getLogLevel();
        promise.resolve(logLevel);
    }

    @ReactMethod
    public void setCustomProperties(ReadableMap properties) {
        MobileCenter.setCustomProperties(RNUtils.toCustomProperties(properties));
    }
}
