package com.microsoft.azure.mobile.react.analytics;

import android.app.Application;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;

import com.microsoft.azure.mobile.react.mobilecenter.RNMobileCenter;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.analytics.Analytics;

import org.json.JSONException;

public class RNAnalyticsModule extends BaseJavaModule {
    public RNAnalyticsModule(Application application, boolean startEnabled) {
        RNMobileCenter.configureMobileCenter(application);
        if (!startEnabled) {
            // Avoid starting an analytics session.
            // Note that we don't call this if startEnabled is true, because
            // that causes a session to try and start before Analytics is started.
            Analytics.setEnabled(startEnabled);
        }
        //Analytics.setAutoPageTrackingEnabled(false); // TODO: once the underlying SDK supports this, make sure to call this
        MobileCenter.start(Analytics.class);
    }

    @Override
    public String getName() {
        return "RNAnalytics";
    }

    @ReactMethod
    public void setEnabled(boolean shouldEnable) {
        Analytics.setEnabled(shouldEnable);
    }

    @ReactMethod
    public void isEnabled(Promise promise) {
        promise.resolve(Analytics.isEnabled());
    }

    @ReactMethod
    public void trackEvent(String eventName, ReadableMap properties, Promise promise) {
        try {
            Analytics.trackEvent(eventName, RNUtils.convertReadableMapToStringMap(properties));
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