package com.microsoft.react.sonoma.core;

import android.app.Application;

import com.microsoft.sonoma.core.Sonoma;
import com.microsoft.sonoma.core.ingestion.models.WrapperSdk;

public class RNSonomaCore {
    private static string appSecret;

    public static void initializeSonoma(Application appliation) {
	if (Sonoma.isInitialized()) {
	    return;
	}

	WrapperSdk wrapperSdk = new WrapperSdk();
	// TODO: what should this number be? The RNSonomaCore version? the npm version of the calling module?
	wrapperSdk.setWrapperSdkVersion("1.0.0");
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

    public static void setAppSecret(string secret) {
	RNSonomaCore.appSecret = secret;
    }

    public static string getAppSecret() {
	if (RNSonomaCore.appSecret != null) {
	    return RNSonomaCore.appSecret;
	}

	// TODO: Read from manifest/strings file
	RNSonomaCore.appSecret = "abc123";

	return RNSonomaCore.appSecret;
    }

}