package com.sonomademoapp;

import android.os.Bundle;

import com.facebook.react.ReactActivity;
import com.microsoft.sonoma.analytics.Analytics;
import com.microsoft.sonoma.core.Sonoma;
import com.microsoft.sonoma.core.utils.UUIDUtils;
import com.microsoft.sonoma.crashes.Crashes;

public class MainActivity extends ReactActivity {

    /**
     * Returns the name of the main component registered from JavaScript.
     * This is used to schedule rendering of the component.
     */
    @Override
    protected String getMainComponentName() {
        return "SonomaDemoApp";
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }
}
