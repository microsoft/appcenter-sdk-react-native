package com.microsoft.azure.mobile.react.analytics;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.analytics.Analytics;
import com.microsoft.azure.mobile.react.mobilecentershared.RNMobileCenterShared;
import com.microsoft.azure.mobile.utils.async.MobileCenterConsumer;

import org.json.JSONException;

@SuppressWarnings("WeakerAccess")
public class RNAnalyticsModule extends BaseJavaModule {

    public RNAnalyticsModule(Application application, boolean startEnabled) {
        RNMobileCenterShared.configureMobileCenter(application);
        if (!startEnabled) {
            // Avoid starting an analytics session.
            // Note that we don't call this if startEnabled is true, because
            // that causes a session to try and start before Analytics is started.
            Analytics.setEnabled(false);
        }
        //Analytics.setAutoPageTrackingEnabled(false); // TODO: once the underlying SDK supports this, make sure to call this
        MobileCenter.start(Analytics.class);
    }

    @Override
    public String getName() {
        return "RNAnalytics";
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
