package com.microsoft.azure.mobile.react.push;

import android.app.Application;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;

import com.microsoft.azure.mobile.react.mobilecenter.RNMobileCenter;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.push.Push;

import org.json.JSONException;

public class RNPushModule extends BaseJavaModule {
    public RNPushModule(Application application) {
        RNMobileCenter.configureMobileCenter(application);

        Push.enableFirebaseAnalytics(application); //TODO: ask user if he wanted to enable it by default?
        //TODO: add listener before to be notified whenver a push notification recieved?
        MobileCenter.start(Push.class);
    }

    @Override
    public String getName() {
        return "RNPush";
    }

    @ReactMethod
    public void setEnabled(boolean shouldEnable) {
        Push.setEnabled(shouldEnable);
    }

    @ReactMethod
    public void isEnabled(Promise promise) {
        promise.resolve(Push.isEnabled());
    }
}
