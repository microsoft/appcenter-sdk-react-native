package com.microsoft.react.sonoma.core;

import android.util.Log;

import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.ReactContextBaseJavaModule;
import com.facebook.react.bridge.ReactMethod;
import com.microsoft.sonoma.core.Sonoma;

import java.util.HashMap;
import java.util.Map;

public class RNSonomaHubModule extends ReactContextBaseJavaModule {
    public RNSonomaHubModule(ReactApplicationContext reactContext) {
        super(reactContext);
    }

    @Override
    public String getName() {
        return "RNSonomaHub";
    }

    @ReactMethod
    public void setEnabled(boolean isEnabled, Promise promise) {
        Sonoma.setEnabled(isEnabled);
        promise.resolve("");
    }

    @ReactMethod
    public void isEnabled(Promise promise) {
        promise.resolve(Sonoma.isEnabled());
    }

    @ReactMethod
    public void getLogLevel(Promise promise) {
        promise.resolve(Sonoma.getLogLevel());
    }

    @ReactMethod
    public void setLogLevel(int logLevel, Promise promise) {
        Sonoma.setLogLevel(logLevel);
        promise.resolve("");
    }

    @ReactMethod
    public void getInstallId(Promise promise) {
        promise.resolve(Sonoma.getInstallId());
    }

    @Override
    public Map<String, Object> getConstants() {
        Map<String, Object> constants = new HashMap<>();
        constants.put("LogLevelAssert", Log.ASSERT);
        constants.put("LogLevelError", Log.ERROR);
        constants.put("LogLevelWarning", Log.WARN);
        constants.put("LogLevelDebug", Log.DEBUG);
        constants.put("LogLevelVerbose", Log.VERBOSE);

        return constants;
    }
}
