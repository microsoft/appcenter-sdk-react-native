// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.testapp;

import android.app.Application;
import android.util.Log;

import com.facebook.react.PackageList;
import com.facebook.react.ReactApplication;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.soloader.SoLoader;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.auth.Auth;
import com.microsoft.appcenter.data.Data;

import java.util.List;

public class MainApplication extends Application implements ReactApplication {

    private final ReactNativeHost mReactNativeHost = new ReactNativeHost(this) {

        @Override
        public boolean getUseDeveloperSupport() {
            return BuildConfig.DEBUG;
        }

        @Override
        protected List<ReactPackage> getPackages() {
            List<ReactPackage> packages = new PackageList(this).getPackages();
            packages.add(new TestAppNativePackage());
            return packages;
        }

        @Override
        protected String getJSMainModuleName() {
            return "index";
        }
    };

    @Override
    public ReactNativeHost getReactNativeHost() {
        return mReactNativeHost;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        AppCenter.setLogLevel(Log.VERBOSE);
        AppCenter.setLogUrl("https://in-integration.dev.avalanch.es");
        Data.setTokenExchangeUrl("https://token-exchange-mbaas-integration.dev.avalanch.es/v0.1");
        Auth.setConfigUrl("https://config-integration.dev.avalanch.es");
        TestAppNativeModule.initSecrets(this);
        SoLoader.init(this, /* native exopackage */ false);
    }
}
