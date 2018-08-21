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

    private static String sAppSecret;

    private static Application sApplication;

    private static WrapperSdk sWrapperSdk = new WrapperSdk();

    @SuppressWarnings("unused")
    public static void configureAppCenter(Application application) {
        if (AppCenter.isConfigured()) {
            return;
        }
        AppCenterReactNativeShared.sApplication = application;
        AppCenterReactNativeShared.sWrapperSdk.setWrapperSdkVersion(com.microsoft.appcenter.reactnative.shared.BuildConfig.VERSION_NAME);
        AppCenterReactNativeShared.sWrapperSdk.setWrapperSdkName(com.microsoft.appcenter.reactnative.shared.BuildConfig.SDK_NAME);
        AppCenter.setWrapperSdk(sWrapperSdk);

        /* Get app secret from appcenter-config.json file. */
        String appSecret = AppCenterReactNativeShared.getAppSecret();
        if (TextUtils.isEmpty(appSecret)) {

            /* No app secret is a special case in SDK where there is no default transmission target. */
            AppCenterLog.debug(LOG_TAG, "Configure without secret");
            AppCenter.configure(application);
        } else {
            AppCenterLog.debug(LOG_TAG, "Configure with secret");
            AppCenter.configure(application, appSecret);
        }
    }

    /**
     * This functionality is intended to allow individual react-native App Center beacons to
     * set specific components of the wrapperSDK cooperatively
     * E.g. code push can fetch the wrapperSdk, set the code push version, then set the
     * wrapperSdk again so it can take effect.
     */
    public static void setWrapperSdk(WrapperSdk wrapperSdk) {
        AppCenterReactNativeShared.sWrapperSdk = wrapperSdk;
        AppCenter.setWrapperSdk(wrapperSdk);
    }

    public static WrapperSdk getWrapperSdk() {
        return AppCenterReactNativeShared.sWrapperSdk;
    }

    public static void setAppSecret(String secret) {
        AppCenterReactNativeShared.sAppSecret = secret;
    }

    public static String getAppSecret() {
        if (AppCenterReactNativeShared.sAppSecret == null) {
            try {
                InputStream configStream = AppCenterReactNativeShared.sApplication.getAssets().open("appcenter-config.json");
                int size = configStream.available();
                byte[] buffer = new byte[size];

                //noinspection ResultOfMethodCallIgnored
                configStream.read(buffer);
                configStream.close();
                String jsonContents = new String(buffer, "UTF-8");
                JSONObject json = new JSONObject(jsonContents);
                AppCenterReactNativeShared.sAppSecret = json.getString("app_secret");
            } catch (Exception e) {
                AppCenterLog.error(LOG_TAG, "Failed to parse appcenter-config.json", e);
            }
        }
        return AppCenterReactNativeShared.sAppSecret;
    }
}
