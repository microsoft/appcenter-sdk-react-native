package com.testapp;

import android.content.Context;
import android.content.SharedPreferences;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;

public class TestAppSecretStringHelperModule extends BaseJavaModule {

    private static final String APP_SECRET = "app_secret";

    private static final String START_AUTOMATICALLY = "start_automatically";

    private final SharedPreferences mSharedPreferences;

    TestAppSecretStringHelperModule(Context context) {
        mSharedPreferences = context.getSharedPreferences(getName(), Context.MODE_PRIVATE);
        String secretOverride = mSharedPreferences.getString(APP_SECRET, null);
        AppCenterReactNativeShared.setAppSecret(secretOverride);
        boolean startAutomaticallyOverride = mSharedPreferences.getBoolean(START_AUTOMATICALLY, true);
        AppCenterReactNativeShared.setStartedAutomatically(startAutomaticallyOverride);
    }

    @Override
    public String getName() {
        return "TestAppSecretStringHelper";
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
}
