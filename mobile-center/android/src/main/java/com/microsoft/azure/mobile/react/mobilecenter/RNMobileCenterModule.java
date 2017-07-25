package com.microsoft.azure.mobile.react.mobilecenter;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.react.mobilecentershared.RNMobileCenterShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

import java.util.UUID;

@SuppressWarnings("WeakerAccess")
public class RNMobileCenterModule extends BaseJavaModule {

    public RNMobileCenterModule(Application application) {
        RNMobileCenterShared.configureMobileCenter(application);
    }

    @Override
    public String getName() {
        return "RNMobileCenter";
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        MobileCenter.setEnabled(enabled).thenAccept(new MobileCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        MobileCenter.isEnabled().thenAccept(new MobileCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
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
    public void getInstallId(final Promise promise) {
        MobileCenter.getInstallId().thenAccept(new MobileCenterConsumer<UUID>() {

            @Override
            public void accept(UUID installId) {
                promise.resolve(installId == null ? null : installId.toString());
            }
        });
    }

    @ReactMethod
    public void setCustomProperties(ReadableMap properties) {
        MobileCenter.setCustomProperties(RNUtils.toCustomProperties(properties));
    }
}
