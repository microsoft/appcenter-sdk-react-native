// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.shared;

import android.app.Application;
import android.text.TextUtils;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.ingestion.models.WrapperSdk;
import com.microsoft.appcenter.utils.AppCenterLog;

import org.json.JSONObject;

import java.io.InputStream;

import static com.microsoft.appcenter.utils.AppCenterLog.LOG_TAG;

public class AppCenterReactNativeShared {

    private static final String APPCENTER_CONFIG_ASSET = "appcenter-config.json";

    private static final String APP_SECRET_KEY = "app_secret";

    private static final String START_AUTOMATICALLY_KEY = "start_automatically";

    private static Application sApplication;

    private static JSONObject sConfiguration;

    private static String sAppSecret;

    private static boolean sStartAutomatically;

    @SuppressWarnings("unused")
    public static synchronized void configureAppCenter(Application application) {
        if (sApplication != null) {
            return;
        }
        sApplication = application;
        WrapperSdk wrapperSdk = new WrapperSdk();
        wrapperSdk.setWrapperSdkVersion(com.microsoft.appcenter.reactnative.shared.BuildConfig.VERSION_NAME);
        wrapperSdk.setWrapperSdkName(com.microsoft.appcenter.reactnative.shared.BuildConfig.SDK_NAME);
        AppCenter.setWrapperSdk(wrapperSdk);
        readConfigurationFile();
        if (!sStartAutomatically) {
            AppCenterLog.debug(LOG_TAG, "Configure not to start automatically.");
            return;
        }

        /* Get app secret from appcenter-config.json file. */
        if (TextUtils.isEmpty(sAppSecret)) {

            /* No app secret is a special case in SDK where there is no default transmission target. */
            AppCenterLog.debug(LOG_TAG, "Configure without secret.");
            AppCenter.configure(application);
        } else {
            AppCenterLog.debug(LOG_TAG, "Configure with secret.");
            AppCenter.configure(application, sAppSecret);
        }
    }

    private static void readConfigurationFile() {
        try {
            AppCenterLog.debug(LOG_TAG, "Reading " + APPCENTER_CONFIG_ASSET);
            InputStream configStream = sApplication.getAssets().open(APPCENTER_CONFIG_ASSET);
            int size = configStream.available();
            byte[] buffer = new byte[size];

            //noinspection ResultOfMethodCallIgnored
            configStream.read(buffer);
            configStream.close();
            String jsonContents = new String(buffer, "UTF-8");
            sConfiguration = new JSONObject(jsonContents);
            if (sAppSecret == null) {
                sAppSecret = sConfiguration.optString(APP_SECRET_KEY);
                sStartAutomatically = sConfiguration.optBoolean(START_AUTOMATICALLY_KEY, true);
            }
        } catch (Exception e) {
            AppCenterLog.error(LOG_TAG, "Failed to parse appcenter-config.json", e);
            sConfiguration = new JSONObject();
        }
    }

    public static synchronized void setAppSecret(String secret) {
        sAppSecret = secret;
    }

    public static synchronized void setStartAutomatically(boolean startAutomatically) {
        sStartAutomatically = startAutomatically;
    }

    public static synchronized JSONObject getConfiguration() {
        return sConfiguration;
    }
}
