using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    /// <summary>
    /// Crashes service.
    /// </summary>
    public partial class Crashes
    {
        static Crashes()
        {
            PlatformCrashes.SendingErrorReport += (sender, e) =>
            {
                SendingErrorReport?.Invoke(sender, e);
            };

            PlatformCrashes.SentErrorReport += (sender, e) =>
            {
                SentErrorReport?.Invoke(sender, e);
            };

            PlatformCrashes.FailedToSendErrorReport += (sender, e) => 
            {
                FailedToSendErrorReport?.Invoke(sender, e);
            };

            PlatformCrashes.ShouldProcessErrorReport = null;
            //PlatformCrashes.GetErrorAttachment = null;
            PlatformCrashes.ShouldAwaitUserConfirmation = null;

            /* 
             * We need to add [Android.Runtime.Preserve] to BindingType to avoid it
             * from being removed by "Link all assemblies optimization".
             * However we cannot do it because this code is shared with ios and PCL.
             * So instead we use the property explicitly here to preserve the method call even after optimization.
             */
            var type = BindingType;
        }

        internal Crashes()
        {
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report is about to be sent.
        /// </summary>
#endif
        public static event SendingErrorReportEventHandler SendingErrorReport;

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report has been successfully sent.
        /// </summary>
#endif
        public static event SentErrorReportEventHandler SentErrorReport;

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report has failed to be sent.
        /// </summary>
#endif
        public static event FailedToSendErrorReportEventHandler FailedToSendErrorReport;

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Set this callback to add custom behavior for determining whether an error report should be processed.
        /// Returning false prevents the crash from being reported to the server.
        /// </summary>
#endif
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            set
            {
                PlatformCrashes.ShouldProcessErrorReport = value;
            }
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Set this callback to add custom behavior for determining whether user confirmation is required to send
        /// error reports.
        /// </summary>
        /// <seealso cref="ShouldAwaitUserConfirmationCallback"/>
#endif
        public static ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation
        {
            set
            {
                PlatformCrashes.ShouldAwaitUserConfirmation = value;
            }
        }

        ///// <summary>
        ///// Set this callback to attach custom text and/or binaries to an error report.
        ///// </summary>
        //public static GetErrorAttachmentCallback GetErrorAttachment
        //{
        //    set
        //    {
        //        PlatformCrashes.GetErrorAttachment = value;
        //    }
        //}

        private static readonly IPlatformCrashes PlatformCrashes = new PlatformCrashes();

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Notifies SDK with a confirmation to handle the crash report.
        /// </summary>
        /// <param name="confirmation">A user confirmation. See <see cref="UserConfirmation"/>.</param>
#endif
        public static void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            PlatformCrashes.NotifyUserConfirmation(confirmation);
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The target SDK Crashes bindings type.
        /// </value>
#endif
        public static Type BindingType => PlatformCrashes.BindingType;

#if WINDOWS_UWP

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
/// <summary>
/// Enables or disables Crashes module.
/// </summary>
#endif
        public static bool Enabled
        {
            get { return PlatformCrashes.Enabled; }
            set { PlatformCrashes.Enabled = value; }
        }

#if WINDOWS_UWP

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
/// <summary>
/// Provides information whether the app crashed in its last session.
/// </summary>
/// <value>
/// <c>true</c> if a crash was recorded in the last session, otherwise <c>false</c>.
/// </value>
#endif
        public static bool HasCrashedInLastSession => PlatformCrashes.HasCrashedInLastSession;

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Gets the crash report generated in the last session if there was a crash.
        /// </summary>
        /// <value>Crash report from the last session, <c>null</c> if there was no crash in the last session.</value>
#endif
        public static async Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return await PlatformCrashes.GetLastSessionCrashReportAsync();
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Generates crash for testing purposes.
        /// </summary>
        /// <remarks>
        /// This call has no effect in non debug configuration (such as release).
        /// </remarks>
#endif
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

