package com.microsoft.sonoma.react.crashes;

import com.microsoft.sonoma.crashes.model.ErrorReport;

public class RNSonomaCrashesListenerAlwaysSend extends RNSonomaCrashesListenerBase {

    public RNSonomaCrashesListenerAlwaysSend() {
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
