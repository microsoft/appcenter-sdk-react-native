package com.microsoft.react.sonoma.analytics;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.sonoma.analytics.Analytics;

import org.json.JSONException;

public class RNSonomaAnalyticsModule extends ReactContextBaseJavaModule {
    public RNSonomaAnalyticsModule(ReactApplicationContext reactContext) {
        super(reactContext);
    }

    @Override
    public String getName() {
        return "RNSonomaAnalytics";
    }

    @ReactMethod
    public void trackEvent(String eventName, ReadableMap properties, Promise promise) {
        try {
            Analytics.trackEvent(eventName, RNSonomaUtils.convertReadableMapToStringMap(properties));
            promise.resolve("");
        } catch (JSONException e) {
            promise.reject(e);
        }
    }

    @ReactMethod
    public void trackPage(String eventName, ReadableMap properties, Promise promise) {
        try {
            Analytics.trackPage(eventName, RNSonomaUtils.convertReadableMapToStringMap(properties));
            promise.resolve("");
        } catch (JSONException e) {
            promise.reject(e);
        }
    }
}