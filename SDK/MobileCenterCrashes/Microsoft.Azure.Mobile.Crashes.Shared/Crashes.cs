using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Crashes feature.
    /// </summary>
    public static class Crashes
    {
        static Crashes()
        {
            PlatformCrashes.SendingErrorReport += (sender, e) =>
            {
                if (SendingErrorReport != null)
                {
                    SendingErrorReport(sender, e);
                }
            };

            PlatformCrashes.SentErrorReport += (sender, e) =>
            {
                if (SentErrorReport != null)
                {
                    SentErrorReport(sender, e);
                }
            };

            PlatformCrashes.FailedToSendErrorReport += (sender, e) => 
            {
                if (FailedToSendErrorReport != null)
                {
                    FailedToSendErrorReport(sender, e);
                }
            };

            PlatformCrashes.ShouldProcessErrorReport = null;
            PlatformCrashes.GetErrorAttachment = null;
            PlatformCrashes.ShouldAwaitUserConfirmation = null;
        }

        /// <summary>
        /// Occurs when an error report is about to be sent.
        /// </summary>
        public static event SendingErrorReportHandler SendingErrorReport;

        /// <summary>
        /// Occurs when an error report has been successfully sent.
        /// </summary>
        public static event SentErrorReportHandler SentErrorReport;

        /// <summary>
        /// Occurs when an error report has failed to be sent.
        /// </summary>
        public static event FailedToSendErrorHandler FailedToSendErrorReport;

        /// <summary>
        /// Set this callback to add custom behavior for determining whether an error report should be processed.
        /// </summary>
        /// <seealso cref="ShouldProcessErrorReportCallback"/>
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            set
            {
                PlatformCrashes.ShouldProcessErrorReport = value;
            }
        }

        /// <summary>
        /// Set this callback to add custom behavior for determining whether user confirmation is required to send
        /// error reports.
        /// </summary>
        /// <seealso cref="ShouldAwaitUserConfirmationCallback"/>
        public static ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation
        {
            set
            {
                PlatformCrashes.ShouldAwaitUserConfirmation = value;
            }
        }

        /// <summary>
        /// Set this callback to add custom behavior for associating an error attachment with an error report.
        /// </summary>
        public static GetErrorAttachmentCallback GetErrorAttachment
        {
            set
            {
                PlatformCrashes.GetErrorAttachment = value;
            }
        }

        internal const string LogTag = MobileCenterLog.LogTag + "Crashes";

        private static readonly IPlatformCrashes PlatformCrashes = new PlatformCrashes();

        /// <summary>
        /// Notifies SDK with a confirmation to handle the crash report.
        /// </summary>
        /// <param name="confirmation">A user confirmation. See <see cref="UserConfirmation"/>.</param>
        public static void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            PlatformCrashes.NotifyUserConfirmation(confirmation);
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The target SDK Crashes bindings type.
        /// </value>
        public static Type BindingType => PlatformCrashes.BindingType;

        /// <summary>
        /// Enable or disable Crashes module.
        /// </summary>
        public static bool Enabled
        {
            get { return PlatformCrashes.Enabled; }
            set { PlatformCrashes.Enabled = value; }
        }

        /// <summary>
        /// Provides information whether the app crashed in its last session.
        /// </summary>
        /// <value>
        /// <c>true</c> if a crash was recorded in the last session, otherwise <c>false</c>.
        /// </value>
        public static bool HasCrashedInLastSession => PlatformCrashes.HasCrashedInLastSession;

        public static ErrorReport LastSessionCrashReport => PlatformCrashes.LastSessionCrashReport;

        /// <summary>
        /// Generates crash for test purpose.
        /// </summary>
        /// <remarks>
        /// This call has no effect in non debug configuration (such as release).
        /// </remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void GenerateTestCrash()
        {
            PlatformCrashes.GenerateTestCrash();
        }

        ///// <summary>
        ///// Track an exception.
        ///// </summary>
        ///// <param name="exception">An exception.</param>
        //public static void TrackException(Exception exception)
        //{
        //    PlatformCrashes.TrackException(exception);
        //}
    }
}
