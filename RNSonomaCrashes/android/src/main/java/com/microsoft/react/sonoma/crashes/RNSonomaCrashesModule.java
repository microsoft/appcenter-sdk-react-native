package com.microsoft.react.sonoma.crashes;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.WritableNativeMap;
import com.microsoft.sonoma.crashes.Crashes;
import com.microsoft.sonoma.crashes.model.ErrorReport;

import org.json.JSONException;

import java.io.IOException;

public class RNSonomaCrashesModule< CrashListenerType extends RNSonomaCrashesListenerBase > extends ReactContextBaseJavaModule {
    private mReactApplicationContext;
    private CrashListenerType mCrashListener;

    private static final HasCrashedInLastSessionKey = "hasCrashedInLastSession";
    private static final LastErrorKey = "lastError";

    public RNSonomaCrashesModule(ReactApplicationContext reactContext, CrashListenerType crashListener) {
        super(reactContext);
	this.mCrashListener = crashListener;
	if (crashListener != null) {
	    Crashes.setListener(crashListener);
	}
    }

    @Override
    public String getName() {
        return "RNSonomaCrashes";
    }
    @Override
    public Map<String, Object> getConstants() {
	final Map<String, Object> constants = new HashMap<>();

	ErrorReport lastError = Crashes.getLastSessionCrashReport();
	
	constants.put(RNSonomaCrashesModule.HasCrashedInLastSessionKey, lastError != null);
	if (lastError) {
	    constants.put(RNSonomaCrashesModule.LastErrorKey, RNSonomaCrashesUtils.convertErrorReportToWritableMap(lastError));
	}
	return constants;
    }

    @ReactMethod
    public void generateTestCrash(final Promise promise) {
        new Thread(new Runnable() {
            public void run() {
                Crashes.generateTestCrash();
                promise.reject(new Exception("generateTestCrash failed to generate a crash"));
            }
        }).start();
    }

    @ReactMethod
    public void sendCrash(Promise promise) {
	if (mCrashListener != null) {
	    mCrashListener.reportUserResponse(Crashes.SEND);
	}
        Crashes.notifyUserConfirmation(Crashes.SEND);
        promise.resolve("");
    }

    @ReactMethod
    public void ignoreCrash(Promise promise) {
	if (mCrashListener != null) {
	    mCrashListener.reportUserResponse(Crashes.DONT_SEND);
	}
        Crashes.notifyUserConfirmation(Crashes.DONT_SEND);
        promise.resolve("");
    }

    @ReactMethod
    public void setTextAttachment(String textAttachment, Promise promise) {
        ErrorReport lastSessionCrashReport = Crashes.getLastSessionCrashReport();
        try {
            RNSonomaErrorAttachmentHelper.saveTextAttachment(getReactApplicationContext(), lastSessionCrashReport, textAttachment);
            promise.resolve("");
        } catch (IOException e) {
            promise.reject(e);
        }
    }
}
