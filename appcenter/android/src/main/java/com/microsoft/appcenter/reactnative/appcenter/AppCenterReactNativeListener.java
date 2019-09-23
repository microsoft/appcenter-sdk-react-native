// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.appcenter;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.modules.core.DeviceEventManagerModule;
import com.microsoft.appcenter.AuthTokenCallback;
import com.microsoft.appcenter.AuthTokenListener;

public class AppCenterReactNativeListener implements AuthTokenListener {

    private static final String ON_ACQUIRE_AUTH_TOKEN_EVENT = "OnAcquireAuthTokenEvent";

    private ReactApplicationContext mReactApplicationContext;

    private AuthTokenCallback mCallback;

    private boolean mAuthTokenEventSent = false;

    private boolean mHasPendingEvent = false;

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
            if (mHasPendingEvent) {
                mReactApplicationContext
                        .getJSModule(DeviceEventManagerModule.RCTDeviceEventEmitter.class)
                        .emit(eventType, null);
                mAuthTokenEventSent = true;
            } else {
                mHasPendingEvent = true;
            }
        }
    }

    // Replay method is only called when JavaScript SetAuthTokenListener is registered.
    void replayPendingEvents() {
        if (!mAuthTokenEventSent) {
            sendEvent(ON_ACQUIRE_AUTH_TOKEN_EVENT);
        }
    }

    AuthTokenCallback getAuthTokenCallback() {
        return mCallback;
    }

    void setAuthTokenCallback(AuthTokenCallback authTokenCallback) {
        mCallback = authTokenCallback;
    }
}