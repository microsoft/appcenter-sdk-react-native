package com.microsoft.react.sonoma.crashes;

import com.facebook.react.bridge.ReactApplicationContext;
import com.microsoft.sonoma.crashes.model.ErrorReport;

public class RNSonomaCrashesListenerAlwaysSend extends RNSonomaCrashesListenerBase {

    public RNSonomaCrashesListenerAlwaysSend(ReactApplicationContext reactApplicationContext) {
        super(reactApplicationContext);
    }

    @Override
    public boolean shouldProcess(ErrorReport report) {
        // Process all crashes
        return true;
    }

    @Override
    public boolean shouldAwaitUserConfirmation() {
	// Do not wait for confirmation, send crashes immediately
	return false;
    }

}
