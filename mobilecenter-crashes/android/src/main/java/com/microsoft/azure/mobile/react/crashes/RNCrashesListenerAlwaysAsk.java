package com.microsoft.azure.mobile.react.crashes;

import com.microsoft.azure.mobile.crashes.model.ErrorReport;

public class RNCrashesListenerAlwaysAsk extends RNCrashesListenerBase {

    public RNCrashesListenerAlwaysAsk() {
    }

    @Override
    public boolean shouldProcess(ErrorReport report) {
        // Keep this report ready to send over to JS
        this.storeReportForJS(report);

        // Process all crashes by default. JS side can stop a crash from
        // being reported via the user confirmation "DONT_SEND" signal.
        return true;
    }

    @Override
    public boolean shouldAwaitUserConfirmation() {
        // Require user confirmation for all crashes, since this is the
        // only way JS can indicate whether or not a crash should be sent.

        return true;
    }

}
