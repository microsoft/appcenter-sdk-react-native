package com.sonomademoapp;

import android.app.Application;

import com.facebook.react.ReactApplication;
import com.facebook.react.ReactNativeHost;
import com.facebook.react.ReactPackage;
import com.facebook.react.shell.MainReactPackage;
import com.microsoft.react.sonoma.analytics.RNSonomaAnalyticsPackage;
import com.microsoft.react.sonoma.crashes.RNSonomaCrashesPackage;
import com.microsoft.sonoma.analytics.Analytics;
import com.microsoft.sonoma.core.Sonoma;
import com.microsoft.sonoma.core.utils.UUIDUtils;
import com.microsoft.sonoma.crashes.Crashes;

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
          new RNSonomaAnalyticsPackage(),
          new RNSonomaCrashesPackage()
      );

      // Sonoma initialization
      Sonoma.start(MainApplication.this, UUIDUtils.randomUUID().toString(), Crashes.class, Analytics.class);
      return packages;
    }
  };

  @Override
  public ReactNativeHost getReactNativeHost() {
      return mReactNativeHost;
  }
}
