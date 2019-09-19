// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.appcenter;

import com.facebook.react.bridge.LifecycleEventListener;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.appcenter.AuthTokenCallback;
import com.microsoft.appcenter.AuthTokenListener;
import com.microsoft.appcenter.utils.AppCenterLog;

import java.util.AbstractMap;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import static com.microsoft.appcenter.AppCenter.LOG_TAG;

public class AppCenterReactNativeListener implements AuthTokenListener {

    private static final String ON_ACQUIRE_AUTH_TOKEN_EVENT = "OnAcquireAuthTokenEvent";

    private ReactApplicationContext mReactApplicationContext;

    private AuthTokenCallback mCallback;

    private List<String> mPendingEvents = new ArrayList<>();

    @SuppressWarnings("WeakerAccess")
    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public void acquireAuthToken(AuthTokenCallback callback) {
        ReactNativeUtils.logInfo("Auth token acquired.");
        sendEvent(ON_ACQUIRE_AUTH_TOKEN_EVENT);
        mCallback = callback;
    }

    private void sendEvent(String eventType) {
        if (mReactApplicationContext != null) {
            if (mReactApplicationContext.hasActiveCatalystInstance()) {
                mReactApplicationContext
                        .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                        .emit(eventType, null);
            } else {
                mPendingEvents.add(eventType);
            }
        }
    }

    void replayPendingEvents() {
        for (String event: mPendingEvents) {
            sendEvent(event);
        }
        mPendingEvents.clear();
    }

    AuthTokenCallback getAuthTokenCallback() {
        return mCallback;
    }

    void setAuthTokenCallback(AuthTokenCallback authTokenCallback) {
        mCallback = authTokenCallback;
    }
}
