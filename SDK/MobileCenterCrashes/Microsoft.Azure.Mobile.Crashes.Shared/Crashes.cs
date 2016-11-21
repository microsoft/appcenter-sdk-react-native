using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Crashes feature.
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

            PlatformCrashes.ShouldProcessErrorReport = ShouldProcessErrorReport;
            PlatformCrashes.GetErrorAttachment = GetErrorAttachment;
        }

        public static event SendingErrorReportHandler SendingErrorReport;
        public static event SentErrorReportHandler SentErrorReport;
        public static event FailedToSendErrorHandler FailedToSendErrorReport;
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport;
        public static GetErrorAttachmentCallback GetErrorAttachment;

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

