// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.testapp;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.crashes.model.TestCrashException;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;

import java.util.concurrent.atomic.AtomicInteger;

import javax.annotation.Nonnull;

public class TestAppNativeModule extends ReactContextBaseJavaModule {

    private static final String TEST_APP_NATIVE = "TestAppNative";

    static {
        System.loadLibrary("native-lib");
    }

    private static final String APP_SECRET = "app_secret";

    private static final String START_AUTOMATICALLY = "start_automatically";

    private final SharedPreferences mSharedPreferences;

    private native void nativeAllocateLargeBuffer();

    TestAppNativeModule(ReactApplicationContext context) {
        super(context);
        mSharedPreferences = context.getSharedPreferences(getName(), Context.MODE_PRIVATE);
    }

    static void initSecrets(Context context) {
        SharedPreferences sharedPreferences = context.getSharedPreferences(TEST_APP_NATIVE, Context.MODE_PRIVATE);
        String secretOverride = sharedPreferences.getString(APP_SECRET, null);
        AppCenterReactNativeShared.setAppSecret(secretOverride);
        boolean startAutomaticallyOverride = sharedPreferences.getBoolean(START_AUTOMATICALLY, true);
        AppCenterReactNativeShared.setStartAutomatically(startAutomaticallyOverride);
    }

    @Override
    @Nonnull
    public String getName() {
        return TEST_APP_NATIVE;
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
                Log.d("TestApp", "Memory allocated: " + i.addAndGet(128) + "MB");
                handler.post(this);
            }
        });
    }
}
