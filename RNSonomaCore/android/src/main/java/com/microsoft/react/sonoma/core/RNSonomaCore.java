package com.microsoft.react.sonoma.core;

import android.app.Application;

import com.microsoft.sonoma.core.Sonoma;
import com.microsoft.sonoma.core.ingestion.models.WrapperSdk;

public class RNSonomaCore {
    private static String appSecret;

    public static void initializeSonoma(Application application) {
        if (Sonoma.isInitialized()) {
            return;
        }

        WrapperSdk wrapperSdk = new WrapperSdk();
        wrapperSdk.setWrapperSdkVersion("0.1.0"); // NOTE: This number must be manually
        wrapperSdk.setWrapperSdkName("react-native-sonoma");

        try {
            // TODO: add codepush information
            // Class codePushConfig = Class.forName("");
            // wrapperSdk.setLiveUpdateReleaseLabel(codepushReleaseLabel) etc
        } catch (Exception e) {
            // Failed to find/create code push, ignore it
        }

        Sonoma.setWrapperSdk(wrapperSdk);
        Sonoma.initialize(application, RNSonomaCore.getAppSecret());
    }

    public static void setAppSecret(String secret) {
        RNSonomaCore.appSecret = secret;
    }

    public static String getAppSecret() {
        if (RNSonomaCore.appSecret == null) {
            // TODO: Read from manifest/strings file
            RNSonomaCore.appSecret = "123e4567-e89b-12d3-a456-426655440000";
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