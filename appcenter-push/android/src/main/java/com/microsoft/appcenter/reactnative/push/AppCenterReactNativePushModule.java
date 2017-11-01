package com.microsoft.appcenter.reactnative.push;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.push.Push;
import com.microsoft.azure.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativePushModule extends BaseJavaModule {

    private AppCenterReactNativePushEventListener mPushListener;

    public AppCenterReactNativePushModule(Application application) {
        AppCenterReactNativeShared.configureMobileCenter(application);
        this.mPushListener = new AppCenterReactNativePushEventListener();

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
        return "AppCenterReactNativePush";
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Push.setEnabled(enabled).thenAccept(new MobileCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Push.isEnabled().thenAccept(new MobileCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    @ReactMethod
    public void sendAndClearInitialNotification(Promise promise) {
        mPushListener.sendAndClearInitialNotification();
    }
}
