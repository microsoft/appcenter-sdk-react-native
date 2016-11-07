package com.microsoft.azure.mobile.react.analytics;

import android.app.Application;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class RNAnalyticsPackage implements ReactPackage {
    private boolean startEnabled;
    private Application application;
    private static final String ENABLE_IN_JS = "ENABLE_IN_JS";

    public RNAnalyticsPackage(Application application, boolean startEnabled) {
        this.startEnabled = startEnabled;
        this.application = application;
    }

    public RNSonomaAnalyticsPackage(Application application, String startEnabled) {
        if (startEnabled.equals(ENABLE_IN_JS)){
            this.startEnabled = false;
        } else {
            this.startEnabled = true;
        }
        
        this.application = application;
    }


    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        List<NativeModule> modules = new ArrayList<>();
        modules.add(new RNAnalyticsModule(application, reactContext, this.startEnabled));
        return modules;
    }

    @Override
    public List<Class<? extends JavaScriptModule>> createJSModules() {
        return Collections.emptyList();
    }

    @Override
    public List<ViewManager> createViewManagers(ReactApplicationContext reactContext) {
        return Collections.emptyList();
    }
}