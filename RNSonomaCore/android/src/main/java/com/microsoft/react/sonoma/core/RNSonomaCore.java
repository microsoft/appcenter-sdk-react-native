package com.microsoft.react.sonoma.core;

import android.app.Application;

import org.json.JSONObject;

import com.microsoft.sonoma.core.Sonoma;
import com.microsoft.sonoma.core.ingestion.models.WrapperSdk;

public class RNSonomaCore {
    private static String appSecret;
    private static Application application;
    private static WrapperSdk wrapperSdk = new WrapperSdk();

    public static void initializeSonoma(Application application) {
        if (Sonoma.isInitialized()) {
            return;
        }
        RNSonomaCore.application = application;

        RNSonomaCore.wrapperSdk.setWrapperSdkVersion("0.1.0"); // NOTE: This number must be manually updated right now
        RNSonomaCore.wrapperSdk.setWrapperSdkName("react-native-sonoma");

        Sonoma.setWrapperSdk(wrapperSdk);
        Sonoma.initialize(application, RNSonomaCore.getAppSecret());
    }

    /**
        This functionality is intended to allow individual react-native sonoma beacons to
        set specific components of the wrapperSDK cooperatively
        E.g. code push can fetch the wrapperSdk, set the code push version, then set the
        wrapperSdk again so it can take effect.
    */
    public static void setWrapperSdk(WrapperSDK wrapperSdk) {
        RNSonomaCore.wrapperSdk = wrapperSdk;
        Sonoma.setWrapperSdk(wrapperSdk);
    }

    public static WrapperSdk getWrapperSdk() {
        return RNSonomaCore.wrapperSdk;
    }

    public static void setAppSecret(String secret) {
        RNSonomaCore.appSecret = secret;
    }

    public static String getAppSecret() {
        if (RNSonomaCore.appSecret == null) {
            try {
                InputStream configStream = RNSonomaCore.application.getAssets().open("sonoma-config.json");
                int size = configStream.available();
                byte[] buffer = new byte[size];
                configStream.read(buffer);
                configStream.close();
                String jsonContents = new String(buffer, "UTF-8");
                JSONObject json = new JSONObject(jsonContents);
                RNSonomaCore.appSecret = json.getString("app_secret");
            } catch (Exception e) {
                // Unable to read secret from file
                // Leave the secret null so that Sonoma errors out appropriately.
            }
        }

        return RNSonomaCore.appSecret;
    }

    public static void setEnabled(boolean enabled) {
        Sonoma.setEnabled(enabled);
    }

    public static void setLogLevel(int logLevel) {
            Sonoma.setLogLevel(logLevel);
    }

    public static int getLogLevel() {
            return Sonoma.getLogLevel();
    }

}