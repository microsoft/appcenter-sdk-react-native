using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Crashes service.
    /// </summary>
    public static partial class Crashes
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
        }

        /// <summary>
        /// Occurs when an error report is about to be sent.
        /// </summary>
        public static event SendingErrorReportEventHandler SendingErrorReport;

        /// <summary>
        /// Occurs when an error report has been successfully sent.
        /// </summary>
        public static event SentErrorReportEventHandler SentErrorReport;

        /// <summary>
        /// Occurs when an error report has failed to be sent.
        /// </summary>
        public static event FailedToSendErrorReportEventHandler FailedToSendErrorReport;

        /// <summary>
        /// Set this callback to add custom behavior for determining whether an error report should be processed.
        /// Returning false prevents the crash from being reported to the server.
        /// </summary>
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            set
            {
                PlatformCrashes.ShouldProcessErrorReport = value;
            }
        }

        /// <summary>
        /// Set this callback to attach custom text and/or binaries to an error report.
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
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The target SDK Crashes bindings type.
        /// </value>
        public static Type BindingType => PlatformCrashes.BindingType;

        /// <summary>
        /// Enables or disables Crashes module.
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

        /// <summary>
        /// Gets the crash report generated in the last session if there was a crash.
        /// </summary>
        /// <value>Crash report from the last session, <c>null</c> if there was no crash in the last session.</value>
        public static ErrorReport LastSessionCrashReport => PlatformCrashes.LastSessionCrashReport;

        /// <summary>
        /// Generates crash for testing purposes.
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

