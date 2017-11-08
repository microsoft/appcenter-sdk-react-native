package com.microsoft.appcenter.reactnative.analytics;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.analytics.Analytics;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import org.json.JSONException;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativeAnalyticsModule extends BaseJavaModule {

    public AppCenterReactNativeAnalyticsModule(Application application, boolean startEnabled) {
        AppCenterReactNativeShared.configureAppCenter(application);
        AppCenter.start(Analytics.class);
        if (!startEnabled) {
            Analytics.setEnabled(false);
        }
    }

    @Override
    public String getName() {
        return "AppCenterReactNativeAnalytics";
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Analytics.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Analytics.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    @ReactMethod
    public void trackEvent(String eventName, ReadableMap properties, Promise promise) {
        try {
            Analytics.trackEvent(eventName, ReactNativeUtils.convertReadableMapToStringMap(properties));
            promise.resolve("");
        } catch (JSONException e) {
            promise.reject(e);
        }
    }
}
