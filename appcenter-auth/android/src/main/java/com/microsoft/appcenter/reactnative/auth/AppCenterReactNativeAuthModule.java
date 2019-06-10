/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

package com.microsoft.appcenter.reactnative.auth;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.ReactMethod;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.auth.Auth;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;

public class AppCenterReactNativeAuthModule extends BaseJavaModule {

    public AppCenterReactNativeAuthModule(Application application) {
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            AppCenter.start(Auth.class);
        }
    }

    @Override
    public String getName() {
        return "AppCenterReactNativeAuth";
    }
}