package com.microsoft.azure.mobile.react.mobilecenterwrapper;

import android.app.Application;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;

import com.microsoft.azure.mobile.react.mobilecenter.RNMobileCenter;
import com.microsoft.azure.mobile.MobileCenter;

import org.json.JSONException;

public class RNMobileCenterWrapperModule extends BaseJavaModule {
    public RNMobileCenterWrapperModule(Application application) {

    }

    @Override
    public String getName() {
        return "RNMobileCenterWrapper";
    }

    @ReactMethod
    public void setCustomProperties(ReadableMap properties) {
        RNMobileCenter.setCustomProperties(RNUtils.toCustomProperties(properties));
    }
}
