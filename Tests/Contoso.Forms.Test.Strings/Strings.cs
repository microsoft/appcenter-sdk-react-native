namespace Contoso.Forms.Test
{
    public static class  TestStrings
    {
        /* General */
        /* Automation Ids */
        public static readonly string DismissButton = "DismissButton";

        /* TestPage */
        /* Automation Ids */
        public static readonly string GoToTogglePageButton = "GoToTogglePageButton";
        public static readonly string GoToCrashesPageButton = "GoToCrashesPageButton";
        public static readonly string GoToAnalyticsPageButton = "GoToAnalyticsPageButton";
        public static readonly string GoToCrashResultsPageButton = "GoToCrashResultsPageButton";

        /* ToggleStatesPage */
        /* Automation Ids */
        public static readonly string EnableMobileCenterButton = "EnableMobileCenterButton";
        public static readonly string EnableAnalyticsButton = "EnableAnalyticsButton";
        public static readonly string EnableCrashesButton = "EnableCrashesButton";
        public static readonly string DisableMobileCenterButton = "DisableMobileCenterButton";
        public static readonly string DisableCrashesButton = "DisableCrashesButton";
        public static readonly string DisableAnalyticsButton = "DisableAnalyticsButton";
        public static readonly string MobileCenterEnabledLabel = "MobileCenterEnabledLabel";
        public static readonly string AnalyticsEnabledLabel = "AnalyticsEnabledLabel";
        public static readonly string CrashesEnabledLabel = "CrashesEnabledLabel";

        /* Text values */
        public static readonly string MobileCenterEnabledText = "Mobile Center enabled";
        public static readonly string MobileCenterDisabledText = "Mobile Center disabled";
        public static readonly string AnalyticsEnabledText = "Analytics enabled";
        public static readonly string AnalyticsDisabledText = "Analytics disabled";
        public static readonly string CrashesEnabledText = "Crashes enabled";
        public static readonly string CrashesDisabledText = "Crashes disabled";

        /* CrashesPage */
        /* Automation Ids */
        public static readonly string DivideByZeroCrashButton = "DivideByZeroCrashButton";
        public static readonly string GenerateTestCrashButton = "GenerateTestCrashButton";
        public static readonly string CatchNullReferenceButton = "CatchNullReferenceButton";
        public static readonly string CrashWithAggregateExceptionButton = "CrashWithAggregateExceptionButton";
        public static readonly string CrashInsideAsyncTaskButton = "CrashInsideAsyncTaskButton";
        public static readonly string CrashWithInvalidOperationButton = "CrashWithInvalidOperationButton";

        /* CrashResultsPage */
        /* Automation Ids */
        public static readonly string HasCrashedInLastSessionLabel = "HasCrashedInLastSessionLabel";
        public static readonly string SendingErrorReportLabel = "SendingErrorReportLabel";
        public static readonly string SentErrorReportLabel = "SentErrorReportLabel";
        public static readonly string FailedToSendErrorReportLabel = "FailedToSendErrorReportLabel";
        public static readonly string ShouldProcessErrorReportLabel = "ShouldProcessErrorReportLabel";
        public static readonly string ShouldAwaitUserConfirmationLabel = "ShouldAwaitUserConfirmationLabel";
        public static readonly string GetErrorAttachmentLabel = "GetErrorAttachmentLabel";
        public static readonly string ViewLastSessionErrorReportButton = "ViewLastSessionErrorReportButton";

        /* Text values */
        public static readonly string HasCrashedInLastSessionText = "HasCrashedInLastSession == true";
        public static readonly string HasNotCrashedInLastSessionText = "HasCrashedInLastSession == false";
        public static readonly string DidSendingErrorReportText = "SendingErrorReport has occured";
        public static readonly string DidNotSendingErrorReportText = "SendingErrorReport has not occured";
        public static readonly string DidSentErrorReportText = "SentErrorReport has occured";
        public static readonly string DidNotSentErrorReportText = "SentErrorReport has not occured";
        public static readonly string DidFailedToSendErrorReportText = "FailedToSendErrorReport has occured";
        public static readonly string DidNotFailedToSendErrorReportText = "FailedToSendErrorReport has not occured";
        public static readonly string DidShouldProcessErrorReportText = "ShouldProcessErrorReport has occured";
        public static readonly string DidNotShouldProcessErrorReportText = "ShouldProcessErrorReport has not occured";
        public static readonly string DidShouldAwaitUserConfirmationText = "ShouldAwaitUserConfirmation has occured";
        public static readonly string DidNotShouldAwaitUserConfirmationText = "ShouldAwaitUserConfirmation has not occured";
        public static readonly string DidGetErrorAttachmentText = "GetErrorAttachment has occured";
        public static readonly string DidNotGetErrorAttachmentText = "GetErrorAttachment has not occured";

        /* LastSessionErrorReportPage */
        /* Automation Ids */
        public static readonly string ExceptionTypeLabel = "ExceptionTypeLabel";
        public static readonly string ExceptionMessageLabel = "ExceptionMessageLabel";
        public static readonly string AppStartTimeLabel = "AppStartTimeLabel";
        public static readonly string AppErrorTimeLabel = "AppErrorTimeLabel";
        public static readonly string IdLabel = "IdLabel";
        public static readonly string DeviceLabel = "DeviceLabel";
        public static readonly string AndroidDetailsLabel = "AndroidDetailsLabel";
        public static readonly string iOSDetailsLabel = "iOSDetailsLabel";

        /* Text values */
        public static readonly string DeviceReportedText = "Device reported";
        public static readonly string HasiOSDetailsText = "Has iOS details";
        public static readonly string HasAndroidDetailsText = "Has Android details";

        /* AnalyticsPage */
        /* Automation Ids */
        public static readonly string SendEventButton = "SendEventButton";
        public static readonly string AddPropertyButton = "AddPropertyButton";
        public static readonly string ClearPropertiesButton = "ClearPropertiesButton";
        public static readonly string GoToAnalyticsResultsPageButton = "GoToAnalyticsResultsPageButton";

        /* AnalyticsResultsPage */
        /* Automation Ids */
        public static readonly string EventPropertiesLabel = "EventPropertiesLabel";
        public static readonly string EventNameLabel = "EventNameLabel";
    }
}
