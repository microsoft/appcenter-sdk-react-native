// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.shared;

import android.app.Application;
import android.content.Context;
import android.text.TextUtils;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.ingestion.models.WrapperSdk;
import com.microsoft.appcenter.utils.AppCenterLog;
import com.microsoft.appcenter.utils.context.AuthTokenContext;

import org.json.JSONObject;

import java.io.InputStream;

import static com.microsoft.appcenter.utils.AppCenterLog.LOG_TAG;

public class AppCenterReactNativeShared {

    private static final String APPCENTER_CONFIG_ASSET = "appcenter-config.json";

    private static final String APP_SECRET_KEY = "app_secret";

    private static final String START_AUTOMATICALLY_KEY = "start_automatically";

    private static final String AUTH_PROVIDER = "auth_provider";

    private static final String AUTH0 = "Auth0";

    private static final String AAD_B2C = "AADB2C";

    private static final String FIREBASE = "Firebase";

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
        if (sConfiguration == null) {
            readConfigurationFile(application.getApplicationContext());
        }
        WrapperSdk wrapperSdk = new WrapperSdk();
        wrapperSdk.setWrapperSdkVersion(com.microsoft.appcenter.reactnative.shared.BuildConfig.VERSION_NAME);
        wrapperSdk.setWrapperSdkName(com.microsoft.appcenter.reactnative.shared.BuildConfig.SDK_NAME);
        AppCenter.setWrapperSdk(wrapperSdk);
        if (sAppSecret == null) {
            sAppSecret = sConfiguration.optString(APP_SECRET_KEY);
            sStartAutomatically = sConfiguration.optBoolean(START_AUTOMATICALLY_KEY, true);
        }

        /*
         * When sStartAutomatically flag is set to true, every service (analytics/auth/crashes/etc.)
         * will be started by separate AppCenter.start call. If any auth provider is used,
         * we should call doNotResetAuthAfterStart to avoid resetting the auth token.
         */
        if (authProviderExistsInConfigFile()) {
            AuthTokenContext.getInstance().doNotResetAuthAfterStart();
        }
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

    public static synchronized JSONObject readConfigurationFile(Context context) {
        if (sConfiguration != null) {
            return sConfiguration;
        }
        try {
            AppCenterLog.debug(LOG_TAG, "Reading " + APPCENTER_CONFIG_ASSET);

            InputStream configStream = context.getAssets().open(APPCENTER_CONFIG_ASSET);
            int size = configStream.available();
            byte[] buffer = new byte[size];

            //noinspection ResultOfMethodCallIgnored
            configStream.read(buffer);
            configStream.close();
            String jsonContents = new String(buffer, "UTF-8");
            sConfiguration = new JSONObject(jsonContents);
        } catch (Exception e) {
            AppCenterLog.error(LOG_TAG, "Failed to parse appcenter-config.json", e);
            sConfiguration = new JSONObject();
        }
        return sConfiguration;
    }

    public static synchronized void setAppSecret(String secret) {
        sAppSecret = secret;
    }

    public static synchronized void setStartAutomatically(boolean startAutomatically) {
        sStartAutomatically = startAutomatically;
    }

    private static boolean authProviderExistsInConfigFile() {
        String authProviderLowerCase = sConfiguration.optString(AUTH_PROVIDER).toLowerCase();
        return authProviderLowerCase.equals(AUTH0.toLowerCase())
                || authProviderLowerCase.equals(FIREBASE.toLowerCase())
                || authProviderLowerCase.equals(AAD_B2C.toLowerCase());
    }
}