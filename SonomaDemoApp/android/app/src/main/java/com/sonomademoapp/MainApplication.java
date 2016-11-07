package com.demoapp;

import android.app.Application;

import com.facebook.react.ReactApplication;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.react.shell.MainReactPackage;

import com.microsoft.azure.mobile.react.analytics.RNAnalyticsPackage;

import com.microsoft.azure.mobile.react.crashes.RNCrashesPackage;

import java.util.Arrays;
import java.util.List;

public class MainApplication extends Application implements ReactApplication {

  private final ReactNativeHost mReactNativeHost = new ReactNativeHost(this) {
    @Override
    protected boolean getUseDeveloperSupport() {
      return BuildConfig.DEBUG;
    }

    @Override
    protected List<ReactPackage> getPackages() {
      // Create RN native packages
      List<ReactPackage> packages = Arrays.<ReactPackage>asList(
          new MainReactPackage(),
          new RNAnalyticsPackage(MainApplication.this, true),
          new RNCrashesPackage(MainApplication.this, new com.microsoft.azure.mobile.react.crashes.RNCrashesListenerAlwaysAsk())
      );
      return packages;
    }
  };

  @Override
  public ReactNativeHost getReactNativeHost() {
      return mReactNativeHost;
  }
}
