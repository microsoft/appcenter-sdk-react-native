package com.sonomademoapp;

import android.app.Application;

import com.facebook.react.ReactApplication;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.react.shell.MainReactPackage;

import com.microsoft.sonoma.react.analytics.RNSonomaAnalyticsPackage;

import com.microsoft.sonoma.react.crashes.RNSonomaCrashesPackage;

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
          new RNSonomaAnalyticsPackage(MainApplication.this, true),
          new RNSonomaCrashesPackage(MainApplication.this, new com.microsoft.sonoma.react.crashes.RNSonomaCrashesListenerAlwaysAsk())
      );
      return packages;
    }
  };

  @Override
  public ReactNativeHost getReactNativeHost() {
      return mReactNativeHost;
  }
}
