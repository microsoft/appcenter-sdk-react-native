package com.microsoft.azure.mobile.react.crashes;

import android.app.Application;
import android.util.Log;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.JavaScriptModule;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;


public class RNCrashesPackage implements ReactPackage {
    private RNCrashesModule mCrashesModule;
    private static final String CRASH_LISTENER_ASK_JAVASCRIPT = "ASK_JAVASCRIPT";

    public RNCrashesPackage(Application application, RNCrashesListenerBase crashListener) {
        // Construct the module up-front to enable crash reporting ASAP
        RNCrashesUtils.logDebug("Creating crashes module");
        this.mCrashesModule = new RNCrashesModule(application, crashListener);
    }

   public RNCrashesPackage(Application application, String crashListenerType) {
        // Construct the module up-front to enable crash reporting ASAP
        RNCrashesUtils.logDebug("Creating crashes module with crashListener " + crashListenerType);
        RNCrashesListenerBase crashListener;
        if (crashListenerType.equals(CRASH_LISTENER_ASK_JAVASCRIPT)){
            crashListener = new RNCrashesListenerAlwaysAsk();
        } else {
            crashListener = new RNCrashesListenerAlwaysSend();
        }

        this.mCrashesModule = new RNCrashesModule(application, crashListener);
    }

    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        // enable JS integrations from this point
        this.mCrashesModule.setReactApplicationContext(reactContext);

        RNCrashesUtils.logDebug("Returning list containing crashes module");
        List<NativeModule> modules = new ArrayList<NativeModule>();
        modules.add(this.mCrashesModule);
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