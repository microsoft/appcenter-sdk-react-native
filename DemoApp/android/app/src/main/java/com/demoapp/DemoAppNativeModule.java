// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.demoapp;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.crashes.model.TestCrashException;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.analytics.Analytics;

import java.util.concurrent.atomic.AtomicInteger;

import javax.annotation.Nonnull;

public class DemoAppNativeModule extends BaseJavaModule {

    private static final String DEMO_APP_NATIVE = "DemoAppNative";

    static {
        System.loadLibrary("native-lib");
    }

    private static final String APP_SECRET = "app_secret";

    private static final String START_AUTOMATICALLY = "start_automatically";

    private final SharedPreferences mSharedPreferences;

    private native void nativeAllocateLargeBuffer();

    private static final String MANUAL_SESSION_TRACKER_ENABLED_KEY = "manual_session_tracker_enabled";

    DemoAppNativeModule(Context context) {
        mSharedPreferences = context.getSharedPreferences(getName(), Context.MODE_PRIVATE);
    }

    static void initSecrets(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(DEMO_APP_NATIVE, Context.MODE_PRIVATE);
        String secretOverride = sharedPreferences.getString(APP_SECRET, null);
        AppCenterReactNativeShared.setAppSecret(secretOverride);
        boolean startAutomaticallyOverride = sharedPreferences.getBoolean(START_AUTOMATICALLY, true);
        AppCenterReactNativeShared.setStartAutomatically(startAutomaticallyOverride);
    }

    static void initManualSessionTrackerState(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(DEMO_APP_NATIVE, Context.MODE_PRIVATE);
        boolean isManualSessionTrackerEnabled = sharedPreferences.getBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, false);
        if (isManualSessionTrackerEnabled) {
            Analytics.enableManualSessionTracker();
        }
    }

    @Override
    @Nonnull
    public String getName() {
        return DEMO_APP_NATIVE;
    }

    @ReactMethod
    public void saveManualSessionTrackerState(boolean state) {
        mSharedPreferences.edit()
                .putBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, state)
                .apply();
    }

    @ReactMethod
    public void getManualSessionTrackerState(Promise promise) {
        promise.resolve(mSharedPreferences.getBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, false) ? 1 : 0);
    }

    @SuppressWarnings("unused")
    @ReactMethod
    public void configureStartup(String secretString, boolean startAutomatically) {

        /* We need to use empty string in Android for no app secret like in JSON file. */
        if (secretString == null) {
            secretString = "";
        }
        mSharedPreferences.edit()
                .putString(APP_SECRET, secretString)
                .putBoolean(START_AUTOMATICALLY, startAutomatically)
                .apply();
    }

    @SuppressWarnings("unused")
    @ReactMethod
    public void generateTestCrash() {

        /*
         * To crash the test app even in release.
         * We reach this code if Crashes.generateTestCrash detected release mode.
         * We can tell with stack trace whether we used SDK method in debug or this one in release.
         */
        throw new TestCrashException();
    }

    @SuppressWarnings("unused")
    @ReactMethod
    public void produceLowMemoryWarning() {
        final AtomicInteger i = new AtomicInteger(0);
        final Handler handler = new Handler(Looper.getMainLooper());
        handler.post(new Runnable() {
            @Override
            public void run() {
                nativeAllocateLargeBuffer();
                Log.d("DemoApp", "Memory allocated: " + i.addAndGet(128) + "MB");
                handler.post(this);
            }
        });
    }
}
