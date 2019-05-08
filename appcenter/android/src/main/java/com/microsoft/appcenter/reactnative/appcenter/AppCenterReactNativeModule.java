// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.appcenter;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.AppCenterLog;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import java.util.UUID;

import static com.microsoft.appcenter.AppCenter.LOG_TAG;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativeModule extends BaseJavaModule {

    private final Application mApplication;

    public AppCenterReactNativeModule(Application application) {
        mApplication = application;
        AppCenterReactNativeShared.configureAppCenter(application);
    }

    @Override
    public String getName() {
        return "AppCenterReactNative";
    }

    @SuppressWarnings("unchecked")
    @ReactMethod
    public void startFromLibrary(ReadableMap service) {
        String type = service.getString("bindingType");
        try {
            AppCenter.startFromLibrary(mApplication, new Class[]{ Class.forName(type) });
        } catch (ClassNotFoundException e) {
            AppCenterLog.error(LOG_TAG, "Unable to resolve App Center module", e);
        }
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        AppCenter.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        AppCenter.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    @ReactMethod
    public void setLogLevel(int logLevel) {
        AppCenter.setLogLevel(logLevel);
    }

    @ReactMethod
    public void getLogLevel(final Promise promise) {
        int logLevel = AppCenter.getLogLevel();
        promise.resolve(logLevel);
    }

    @ReactMethod
    public void getInstallId(final Promise promise) {
        AppCenter.getInstallId().thenAccept(new AppCenterConsumer<UUID>() {

            @Override
            public void accept(UUID installId) {
                promise.resolve(installId == null ? null : installId.toString());
            }
        });
    }

    @ReactMethod
    public void setUserId(String userId) {
        AppCenter.setUserId(userId);
    }

    @ReactMethod
    public void setCustomProperties(ReadableMap properties) {
        AppCenter.setCustomProperties(ReactNativeUtils.toCustomProperties(properties));
    }
}
