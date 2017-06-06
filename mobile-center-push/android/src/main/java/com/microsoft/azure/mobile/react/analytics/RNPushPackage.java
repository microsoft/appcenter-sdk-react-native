package com.microsoft.azure.mobile.react.push;

import android.app.Application;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class RNPushPackage implements ReactPackage {
    private static final String ENABLE_IN_JS = "ENABLE_IN_JS";
    private RNPushModule module;

    public RNPushPackage(Application application, boolean startEnable) {
        this.module = new RNPushModule(application, startEnable);
    }

    public RNPushPackage(Application application, String startEnabled) {
        if (startEnabled.equals(ENABLE_IN_JS)){
            this.module = new RNPushModule(application, false);
        } else {
            this.module = new RNPushModule(application, true);
        }
    }

    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        List<NativeModule> modules = new ArrayList<>();
        modules.add(this.module);
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
