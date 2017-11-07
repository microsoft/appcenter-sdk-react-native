package com.microsoft.appcenter.reactnative.push;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.push.Push;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativePushModule extends BaseJavaModule {

    private AppCenterReactNativePushEventListener mPushListener;

    public AppCenterReactNativePushModule(Application application) {
        AppCenterReactNativeShared.configureAppCenter(application);
        this.mPushListener = new AppCenterReactNativePushEventListener();

        Push.setListener(mPushListener);
        AppCenter.start(Push.class);
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
        Push.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Push.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

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
