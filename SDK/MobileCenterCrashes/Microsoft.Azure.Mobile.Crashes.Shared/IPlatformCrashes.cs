using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Interface to abstract <see cref="Crashes"/> features between different platforms.
    /// </summary>
    internal interface IPlatformCrashes
    {
        Type BindingType { get; }

        bool Enabled { get; set; }

        bool HasCrashedInLastSession { get; }

        ErrorReport LastSessionCrashReport { get; }

        void GenerateTestCrash();

        void NotifyUserConfirmation(UserConfirmation confirmation);

        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are some corresponding events)
        SendingErrorReportHandler SendingErrorReport { get; set; }
        SentErrorReportHandler SentErrorReport { get; set; }
        FailedToSendErrorHandler FailedToSendErrorReport { get; set; }
        ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }
        GetErrorAttachmentCallback GetErrorAttachment { get; set; }

        //void TrackException(Exception exception);
    }
}
