package com.demoapp;

import android.content.Context;
import android.content.SharedPreferences;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.crashes.model.TestCrashException;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;

public class DemoAppNativeModule extends BaseJavaModule {

    private static final String APP_SECRET = "app_secret";

    private static final String START_AUTOMATICALLY = "start_automatically";

    private final SharedPreferences mSharedPreferences;

    DemoAppNativeModule(Context context) {
        mSharedPreferences = context.getSharedPreferences(getName(), Context.MODE_PRIVATE);
        String secretOverride = mSharedPreferences.getString(APP_SECRET, null);
        AppCenterReactNativeShared.setAppSecret(secretOverride);
        boolean startAutomaticallyOverride = mSharedPreferences.getBoolean(START_AUTOMATICALLY, true);
        AppCenterReactNativeShared.setStartAutomatically(startAutomaticallyOverride);
    }

    @Override
    public String getName() {
        return "DemoAppNative";
    }

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

    @ReactMethod
    public void generateTestCrash() {

        /*
         * To crash the test app even in release.
         * We reach this code if Crashes.generateTestCrash detected release mode.
         * We can tell with stack trace whether we used SDK method in debug or this one in release.
         */
        throw new TestCrashException();
    }
}
