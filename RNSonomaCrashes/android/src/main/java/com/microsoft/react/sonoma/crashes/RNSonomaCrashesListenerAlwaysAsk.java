package com.microsoft.react.sonoma.crashes;

import com.microsoft.sonoma.crashes.model.ErrorReport;

public class RNSonomaCrashesListenerAlwaysAsk extends RNSonomaCrashesListenerBase {

    public RNSonomaCrashesListenerAlwaysAsk() {
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

	// TODO: This function is called ~ when Sonoma.Start(Crashes) is called
	// At that point there is not yet a JS environment. We need to inform JS
	// once it starts that there is a pending error waiting for user interaction
        return true;
    }

}
