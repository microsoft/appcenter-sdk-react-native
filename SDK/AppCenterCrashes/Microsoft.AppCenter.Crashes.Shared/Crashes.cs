#pragma warning disable CS0067 // Event never invoked

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Crashes service.
    /// </summary>
    public partial class Crashes
    {
        /// <summary>
        /// Log tag used by the Crashes component.
        /// </summary>
        public const string LogTag = "AppCenterCrashes";

        // We use the EditorBrowsable attribute to hide the unimplemented APIs from UWP apps.
        // The APIs will still be visible if this is added as a project reference, but otherwise,
        // (so if it's added via nuget), they will be hidden. Unless the VS instance has resharper,
        // which is why we also use the Obsolete attribute.
#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report is about to be sent.
        /// </summary>
#endif
        public static event SendingErrorReportEventHandler SendingErrorReport;

#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report has been successfully sent.
        /// </summary>
#endif
        public static event SentErrorReportEventHandler SentErrorReport;

#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Occurs when an error report has failed to be sent.
        /// </summary>
#endif
        public static event FailedToSendErrorReportEventHandler FailedToSendErrorReport;

#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Set this callback to add custom behavior for determining whether an error report should be processed.
        /// Returning false prevents the crash from being reported to the server.
        /// </summary>
#endif
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport;

#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Set this callback to add custom behavior for determining whether user confirmation is required to send
        /// error reports.
        /// </summary>
        /// <seealso cref="ShouldAwaitUserConfirmationCallback"/>
#endif
        public static ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation;

        /// <summary>
        /// Set this callback to attach custom text and/or binaries to an error report.
        /// </summary>
        public static GetErrorAttachmentsCallback GetErrorAttachments;

#if USES_WATSON
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
            PlatformNotifyUserConfirmation(confirmation);
        }

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
            return PlatformIsEnabledAsync();
        }

#if WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Enable or disable the Crashes service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
#endif
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
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
        public static Task<bool> HasCrashedInLastSessionAsync() => PlatformHasCrashedInLastSessionAsync();

#if USES_WATSON
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
            return PlatformGetLastSessionCrashReportAsync();
        }

        /// <summary>
        /// Generates crash for testing purposes.
        /// </summary>
        /// <remarks>
        /// This call has no effect in non debug configuration (such as release).
        /// </remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void GenerateTestCrash()
        {
            throw new TestCrashException();
        }

#if USES_WATSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This does not exist in UWP and should not be used.")]
#else
        /// <summary>
        /// Track a handled error.
        /// </summary>
        /// <param name="exception">The .NET exception describing the handled error.</param>
        /// <param name="properties">Optional properties.</param>
#endif
        public static void TrackError(Exception exception, IDictionary<string, string> properties = null)
        {
            PlatformTrackError(exception, properties);
        }
    }
}
