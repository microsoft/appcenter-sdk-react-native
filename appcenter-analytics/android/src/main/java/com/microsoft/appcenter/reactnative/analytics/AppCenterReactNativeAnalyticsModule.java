// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.analytics;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.analytics.Analytics;
import com.microsoft.appcenter.analytics.AnalyticsTransmissionTarget;
import com.microsoft.appcenter.analytics.PropertyConfigurator;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.AppCenterLog;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import org.json.JSONException;

import java.util.HashMap;
import java.util.Map;

import static com.microsoft.appcenter.analytics.Analytics.LOG_TAG;

@SuppressWarnings("WeakerAccess")
public class AppCenterReactNativeAnalyticsModule extends BaseJavaModule {

    private Map<String, AnalyticsTransmissionTarget> mTransmissionTargets = new HashMap<>();

    public AppCenterReactNativeAnalyticsModule(Application application, boolean startEnabled) {
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            AppCenter.start(Analytics.class);
            if (!startEnabled) {
                Analytics.setEnabled(false);
            }
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
        } catch (JSONException e) {
            AppCenterLog.error(LOG_TAG, "Could not convert event properties from JavaScript to Java", e);
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void trackTransmissionTargetEvent(String eventName, ReadableMap properties, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            try {
                transmissionTarget.trackEvent(eventName, ReactNativeUtils.convertReadableMapToStringMap(properties));
            } catch (JSONException e) {
                AppCenterLog.error(LOG_TAG, "Could not convert event properties from JavaScript to Java", e);
            }
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void getTransmissionTarget(String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = Analytics.getTransmissionTarget(targetToken);
        if (transmissionTarget == null) {
            promise.resolve(null);
            return;
        }
        mTransmissionTargets.put(targetToken, transmissionTarget);
        promise.resolve(targetToken);
    }

    @ReactMethod
    public void isTransmissionTargetEnabled(String targetToken, final Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget == null) {
            promise.resolve(null);
            return;
        }
        transmissionTarget.isEnabledAsync().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean enabled) {
                promise.resolve(enabled);
            }
        });
    }

    @ReactMethod
    public void setTransmissionTargetEnabled(boolean enabled, String targetToken, final Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget == null) {
            promise.resolve(null);
            return;
        }
        transmissionTarget.setEnabledAsync(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void result) {
                promise.resolve(result);
            }
        });
    }

    @ReactMethod
    public void setTransmissionTargetEventProperty(String propertyKey, String propertyValue, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.setEventProperty(propertyKey, propertyValue);
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void removeTransmissionTargetEventProperty(String propertyKey, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.removeEventProperty(propertyKey);
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void collectTransmissionTargetDeviceId(String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.collectDeviceId();
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void getChildTransmissionTarget(String childToken, String parentToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(parentToken);
        if (transmissionTarget == null) {
            promise.resolve(null);
            return;
        }
        AnalyticsTransmissionTarget childTarget = transmissionTarget.getTransmissionTarget(childToken);
        if (childTarget == null) {
            promise.resolve(null);
            return;
        }
        mTransmissionTargets.put(childToken, childTarget);
        promise.resolve(childToken);
    }
    
    @ReactMethod
    public void setTransmissionTargetAppName(String appName, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.setAppName(appName);
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void setTransmissionTargetAppVersion(String appVersion, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.setAppVersion(appVersion);
        }
        promise.resolve(null);
    }

    @ReactMethod
    public void setTransmissionTargetAppLocale(String appLocale, String targetToken, Promise promise) {
        AnalyticsTransmissionTarget transmissionTarget = mTransmissionTargets.get(targetToken);
        if (transmissionTarget != null) {
            PropertyConfigurator configurator = transmissionTarget.getPropertyConfigurator();
            configurator.setAppLocale(appLocale);
        }
        promise.resolve(null);
    }
}
