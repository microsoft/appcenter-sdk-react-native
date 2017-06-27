package com.microsoft.azure.mobile.react.mobilecentershared;

import android.app.Application;

import java.util.Map;
import java.util.Date;
import org.json.JSONArray;
import org.json.JSONObject;
import org.json.JSONException;

import java.io.InputStream;

import com.microsoft.azure.mobile.CustomProperties;
import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.ingestion.models.WrapperSdk;


public class RNMobileCenterShared {
    private static String appSecret;
    private static Application application;
    private static WrapperSdk wrapperSdk = new WrapperSdk();

    public static void configureMobileCenter(Application application) {
        if (MobileCenter.isConfigured()) {
            return;
        }
        RNMobileCenterShared.application = application;

        RNMobileCenterShared.wrapperSdk.setWrapperSdkVersion(com.microsoft.azure.mobile.react.mobilecentershared.BuildConfig.VERSION_NAME);
        RNMobileCenterShared.wrapperSdk.setWrapperSdkName(com.microsoft.azure.mobile.react.mobilecentershared.BuildConfig.SDK_NAME);

        MobileCenter.setWrapperSdk(wrapperSdk);
        MobileCenter.configure(application, RNMobileCenterShared.getAppSecret());
    }

    /**
        This functionality is intended to allow individual react-native Mobile Center beacons to
        set specific components of the wrapperSDK cooperatively
        E.g. code push can fetch the wrapperSdk, set the code push version, then set the
        wrapperSdk again so it can take effect.
    */
    public static void setWrapperSdk(WrapperSdk wrapperSdk) {
        RNMobileCenterShared.wrapperSdk = wrapperSdk;
        MobileCenter.setWrapperSdk(wrapperSdk);
    }

    public static WrapperSdk getWrapperSdk() {
        return RNMobileCenterShared.wrapperSdk;
    }

    public static void setAppSecret(String secret) {
        RNMobileCenterShared.appSecret = secret;
    }

    public static String getAppSecret() {
        if (RNMobileCenterShared.appSecret == null) {
            try {
                InputStream configStream = RNMobileCenterShared.application.getAssets().open("mobile-center-config.json");
                int size = configStream.available();
                byte[] buffer = new byte[size];
                configStream.read(buffer);
                configStream.close();
                String jsonContents = new String(buffer, "UTF-8");
                JSONObject json = new JSONObject(jsonContents);
                RNMobileCenterShared.appSecret = json.getString("app_secret");
            } catch (Exception e) {
                // Unable to read secret from file
                // Leave the secret null so that Mobile Center errors out appropriately.
            }
        }

        return RNMobileCenterShared.appSecret;
    }
}
