package com.microsoft.appcenter.reactnative.push;

import android.app.Application;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class AppCenterReactNativePushPackage implements ReactPackage {
    private AppCenterReactNativePushModule mPushModule;

    public AppCenterReactNativePushPackage(Application application) {
        this.mPushModule = new AppCenterReactNativePushModule(application);
    }

    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        this.mPushModule.setReactApplicationContext(reactContext);

        List<NativeModule> modules = new ArrayList<>();
        modules.add(this.mPushModule);
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
