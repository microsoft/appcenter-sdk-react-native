// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        public static ShouldProcessErrorReportCallback ShouldProcessErrorReport;

        /// <summary>
        /// Set this callback to add custom behavior for determining whether user confirmation is required to send
        /// error reports.
        /// </summary>
        /// <seealso cref="ShouldAwaitUserConfirmationCallback"/>
        public static ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation;

        /// <summary>
        /// Set this callback to attach custom text and/or binaries to an error report.
        /// </summary>
        public static GetErrorAttachmentsCallback GetErrorAttachments;

        /// <summary>
        /// Notifies SDK with a confirmation to handle the crash report.
        /// </summary>
        /// <param name="confirmation">A user confirmation. See <see cref="UserConfirmation"/>.</param>
        public static void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            PlatformNotifyUserConfirmation(confirmation);
        }

        /// <summary>
        /// Check whether the Crashes service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        /// Enable or disable the Crashes service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Provides information whether the app crashed in its last session.
        /// </summary>
        /// <value>
        /// Task with result being <c>true</c> if a crash was recorded in the last session, otherwise <c>false</c>.
        /// </value>
        public static Task<bool> HasCrashedInLastSessionAsync()
        {
            return PlatformHasCrashedInLastSessionAsync();
        }

        /// <summary>
        /// Gets the crash report generated in the last session if there was a crash.
        /// </summary>
        /// <value>Crash report from the last session, <c>null</c> if there was no crash in the last session.</value>
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

        /// <summary>
        /// Track a handled error.
        /// </summary>
        /// <param name="exception">The .NET exception describing the handled error.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackError(Exception exception, IDictionary<string, string> properties = null)
        {
            if (exception == null)
            {
                AppCenterLog.Error(LogTag, "TrackError exception parameter cannot be null.");
            }
            else
            {
                PlatformTrackError(exception, properties);
            }
        }
    }
}
