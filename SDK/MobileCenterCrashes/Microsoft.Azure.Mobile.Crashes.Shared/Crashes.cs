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
#pragma warning disable 618
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
            PlatformCrashes.GetErrorAttachments = null;
            PlatformCrashes.ShouldAwaitUserConfirmation = null;

            /* 
             * We need to add [Android.Runtime.Preserve] to BindingType to avoid it
             * from being removed by "Link all assemblies optimization".
             * However we cannot do it because this code is shared with ios and PCL.
             * So instead we use the property explicitly here to preserve the method call even after optimization.
             */
            var type = BindingType;
#pragma warning restore 618
        }

        internal Crashes()
        {
        }

        // We use the EditorBrowsable attribute to hide the unimplemented APIs from UWP apps.
        // The APIs will still be visible if this is added as a project reference, but otherwise,
        // (so if it's added via nuget), they will be hidden. Unless the VS instance has resharper,
        // which is why we also use the Obsolete attribute.
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

        /// <summary>
        /// Set this callback to attach custom text and/or binaries to an error report.
        /// </summary>
        public static GetErrorAttachmentsCallback GetErrorAttachments
        {
            set
            {
                PlatformCrashes.GetErrorAttachments = value;
            }
        }

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
        /// Check whether the Crashes service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
#endif
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformCrashes.IsEnabledAsync();
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Enable or disable the Crashes service.
        /// </summary>
#endif
        public static void SetEnabled(bool enabled)
        {
            PlatformCrashes.SetEnabled(enabled);
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Provides information whether the app crashed in its last session.
        /// </summary>
        /// <value>
        /// Task with result being <c>true</c> if a crash was recorded in the last session, otherwise <c>false</c>.
        /// </value>
#endif
        public static Task<bool> HasCrashedInLastSessionAsync() => PlatformCrashes.HasCrashedInLastSessionAsync();

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Gets the crash report generated in the last session if there was a crash.
        /// </summary>
        /// <value>Crash report from the last session, <c>null</c> if there was no crash in the last session.</value>
#endif
        public static Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return PlatformCrashes.GetLastSessionCrashReportAsync();
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

