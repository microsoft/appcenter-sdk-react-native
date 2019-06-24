// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.push;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.push.Push;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.AppCenterLog;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;
import com.microsoft.appcenter.utils.storage.SharedPreferencesManager;

import javax.annotation.Nonnull;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativePushModule extends BaseJavaModule {

    private static final String ENABLE_PUSH_IN_JAVASCRIPT = "enable_push_in_javascript";

    private static final String PUSH_ONCE_ENABLED = "PushOnceEnabled";

    private boolean mPushStarted;

    private AppCenterReactNativePushEventListener mPushListener;

    public AppCenterReactNativePushModule(Application application) {
        this.mPushListener = new AppCenterReactNativePushEventListener();
        Push.setListener(mPushListener);
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            boolean startPush = true;
            boolean enablePushInJavascript = AppCenterReactNativeShared.getConfiguration().optBoolean(ENABLE_PUSH_IN_JAVASCRIPT);
            if (enablePushInJavascript) {

                /*
                 * TODO expose a way to post command in background looper in native SDK to avoid accessing storage directly here.
                 * Storage might not be ready yet so we initialize it here to make sure.
                 */
                SharedPreferencesManager.initialize(application.getApplicationContext());
                startPush = SharedPreferencesManager.getBoolean(PUSH_ONCE_ENABLED);
            }
            if (startPush) {
                AppCenter.start(Push.class);
                mPushStarted = true;
            }
        }
    }

    public void setReactApplicationContext(ReactApplicationContext reactContext) {
        if (this.mPushListener != null) {
            this.mPushListener.setReactApplicationContext(reactContext);
        }
    }

    @Nonnull
    @Override
    public String getName() {
        return "AppCenterReactNativePush";
    }

    @ReactMethod
    public void setEnabled(final boolean enabled, final Promise promise) {
        if (AppCenter.isConfigured()) {
            if (!mPushStarted && enabled) {
                AppCenter.start(Push.class);
                mPushStarted = true;
            }
            Push.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

                @Override
                public void accept(Void result) {
                    if (enabled) {

                        /* TODO expose a way to post command in background looper in native SDK to avoid accessing storage directly here. */
                        SharedPreferencesManager.putBoolean(PUSH_ONCE_ENABLED, true);
                    }
                    promise.resolve(result);
                }
            });
        } else {
            AppCenterLog.error(getName(), "AppCenter needs to be started before Push can be enabled.");
        }
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
