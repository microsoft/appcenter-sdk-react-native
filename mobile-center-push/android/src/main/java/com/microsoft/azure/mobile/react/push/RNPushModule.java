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
    private RNPushEventListener mPushListener;

    public RNPushModule(Application application) {
        RNMobileCenter.configureMobileCenter(application);
        this.mPushListener = new RNPushEventListener();

        Push.setListener(mPushListener);
        MobileCenter.start(Push.class);
    }

    public void setReactApplicationContext(ReactApplicationContext reactContext) {
        if (this.mPushListener != null) {
            this.mPushListener.setReactApplicationContext(reactContext);
        }
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
Contact GitHub API Training Shop Blog About
