/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

package com.microsoft.appcenter.reactnative.auth;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.bridge.WritableNativeMap;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.auth.Auth;
import com.microsoft.appcenter.auth.SignInResult;
import com.microsoft.appcenter.auth.UserInformation;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import javax.annotation.Nonnull;

public class AppCenterReactNativeAuthModule extends BaseJavaModule {

    private static final String ACCESS_TOKEN_KEY = "accessToken";

    private static final String ACCOUNT_ID_KEY = "accountId";

    private static final String ID_TOKEN_KEY = "idToken";

    public AppCenterReactNativeAuthModule(Application application) {
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            AppCenter.start(Auth.class);
        }
    }

    @Nonnull
    @Override
    public String getName() {
        return "AppCenterReactNativeAuth";
    }

    @ReactMethod
    public void isEnabled(final Promise promise) {
        Auth.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean isEnabled) {
                promise.resolve(isEnabled);
            }
        });
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Auth.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void aVoid) {
                promise.resolve(null);
            }
        });
    }

    @ReactMethod
    public void signIn(final Promise promise) {
        Auth.signIn().thenAccept(new AppCenterConsumer<SignInResult>() {

            @Override
            public void accept(SignInResult signInResult) {
                WritableMap writableMap = new WritableNativeMap();
                if (signInResult.getException() == null) {

                    /* Sign-in succeeded, convert Java result to a JavaScript result. */
                    UserInformation userInformation = signInResult.getUserInformation();
                    writableMap.putString(ACCESS_TOKEN_KEY, userInformation.getAccessToken());
                    writableMap.putString(ACCOUNT_ID_KEY, userInformation.getAccountId());
                    writableMap.putString(ID_TOKEN_KEY, userInformation.getIdToken());
                    promise.resolve(writableMap);
                } else {
                    Exception signInFailureException = signInResult.getException();
                    promise.reject("Sign-in failed", signInFailureException);
                }
            }
        });
    }

    @ReactMethod
    public void signOut() {
        Auth.signOut();
    }
}