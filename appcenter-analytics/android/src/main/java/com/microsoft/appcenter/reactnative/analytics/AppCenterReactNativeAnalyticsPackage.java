// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.analytics;

import android.app.Application;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class AppCenterReactNativeAnalyticsPackage implements ReactPackage {
    private static final String ENABLE_IN_JS = "ENABLE_IN_JS";
    private AppCenterReactNativeAnalyticsModule module;

    public AppCenterReactNativeAnalyticsPackage(Application application, boolean startEnabled) {
        // We create the module early because the Analytics package depends on the
        // onResume event to determine whether the app is foregrounded, and so
        // automatic session management doesn't work if we start it too late
        this.module = new AppCenterReactNativeAnalyticsModule(application, startEnabled);
    }

    public AppCenterReactNativeAnalyticsPackage(Application application, String startEnabled) {
        if (startEnabled.equals(ENABLE_IN_JS)){
            this.module = new AppCenterReactNativeAnalyticsModule(application, false);
        } else {
            this.module = new AppCenterReactNativeAnalyticsModule(application, true);
        }

    }


    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        List<NativeModule> modules = new ArrayList<>();
        modules.add(this.module);
        return modules;
    }

    // No @Override to support applications using React Native 0.47.0 or later
    public List<Class<? extends JavaScriptModule>> createJSModules() {
        return Collections.emptyList();
    }

    @Override
    public List<ViewManager> createViewManagers(ReactApplicationContext reactContext) {
        return Collections.emptyList();
    }
}