package com.microsoft.appcenter.reactnative.analytics;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.analytics.Analytics;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

import org.json.JSONException;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativeAnalyticsModule extends BaseJavaModule {

    public AppCenterReactNativeAnalyticsModule(Application application, boolean startEnabled) {
        AppCenterReactNativeShared.configureMobileCenter(application);
        MobileCenter.start(Analytics.class);
        if (!startEnabled) {
            Analytics.setEnabled(false);
        }
        //Analytics.setAutoPageTrackingEnabled(false); // TODO: once the underlying SDK supports this, make sure to call this
    }

    @Override
    public String getName() {
        return "AppCenterReactNativeAnalytics";
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Analytics.setEnabled(enabled).thenAccept(new MobileCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Analytics.isEnabled().thenAccept(new MobileCenterConsumer<Boolean>() {

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

    /*
    // TODO: once the underlying SDK supports this
    @ReactMethod
    public void trackPage(String pageName, ReadableMap properties, Promise promise) {
        try {
            Analytics.trackPage(pageName, RNUtils.convertReadableMapToStringMap(properties));
            promise.resolve("");
        } catch (JSONException e) {
            promise.reject(e);
        }
    }
    */
}
