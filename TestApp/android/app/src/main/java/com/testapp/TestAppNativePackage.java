// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.testapp;

import android.content.Context;

import com.facebook.react.ReactPackage;
import com.facebook.react.bridge.NativeModule;
import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.uimanager.ViewManager;

import java.util.Collections;
import java.util.List;

public class TestAppNativePackage implements ReactPackage {

    private final NativeModule mModule;

    TestAppNativePackage(Context context) {
        mModule = new TestAppNativeModule(context);
    }

    @Override
    public List<NativeModule> createNativeModules(ReactApplicationContext reactContext) {
        return Collections.singletonList(mModule);
    }

    @Override
    public List<ViewManager> createViewManagers(ReactApplicationContext reactContext) {
        return Collections.emptyList();
    }
}
