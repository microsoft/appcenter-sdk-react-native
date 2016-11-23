package com.microsoft.azure.mobile.react.mobilecenter;

import android.app.Application;

import org.json.JSONObject;

import java.io.InputStream;

import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.ingestion.models.WrapperSdk;

public class RNMobileCenter {
    private static String appSecret;
    private static Application application;
    private static WrapperSdk wrapperSdk = new WrapperSdk();

    public static void configureMobileCenter(Application application) {
        if (MobileCenter.isConfigured()) {
            return;
        }
        RNMobileCenter.application = application;

        RNMobileCenter.wrapperSdk.setWrapperSdkVersion(com.microsoft.azure.mobile.react.mobilecenter.BuildConfig.VERSION_NAME);
        RNMobileCenter.wrapperSdk.setWrapperSdkName(com.microsoft.azure.mobile.react.mobilecenter.BuildConfig.SDK_NAME);

        MobileCenter.setWrapperSdk(wrapperSdk);
        MobileCenter.configure(application, RNMobileCenter.getAppSecret());
    }

    /**
        This functionality is intended to allow individual react-native Mobile Center beacons to
        set specific components of the wrapperSDK cooperatively
        E.g. code push can fetch the wrapperSdk, set the code push version, then set the
        wrapperSdk again so it can take effect.
    */
    public static void setWrapperSdk(WrapperSdk wrapperSdk) {
        RNMobileCenter.wrapperSdk = wrapperSdk;
        MobileCenter.setWrapperSdk(wrapperSdk);
    }

    public static WrapperSdk getWrapperSdk() {
        return RNMobileCenter.wrapperSdk;
    }

    public static void setAppSecret(String secret) {
        RNMobileCenter.appSecret = secret;
    }

    public static String getAppSecret() {
        if (RNMobileCenter.appSecret == null) {
            try {
                InputStream configStream = RNMobileCenter.application.getAssets().open("mobile-center-config.json");
                int size = configStream.available();
                byte[] buffer = new byte[size];
                configStream.read(buffer);
                configStream.close();
                String jsonContents = new String(buffer, "UTF-8");
                JSONObject json = new JSONObject(jsonContents);
                RNMobileCenter.appSecret = json.getString("app_secret");
            } catch (Exception e) {
                // Unable to read secret from file
                // Leave the secret null so that Mobile Center errors out appropriately.
            }
        }

        return RNMobileCenter.appSecret;
    }

    public static void setEnabled(boolean enabled) {
        MobileCenter.setEnabled(enabled);
    }

    public static void setLogLevel(int logLevel) {
        MobileCenter.setLogLevel(logLevel);
    }

    public static int getLogLevel() {
        return MobileCenter.getLogLevel();
    }

}