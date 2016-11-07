package com.microsoft.azure.mobile.react.crashes;

import com.microsoft.sonoma.crashes.model.ErrorReport;

public class RNCrashesListenerAlwaysSend extends RNCrashesListenerBase {

    public RNCrashesListenerAlwaysSend() {
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
