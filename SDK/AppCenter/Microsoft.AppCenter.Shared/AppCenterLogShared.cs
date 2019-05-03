// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// This class is used to log messages consistent with those emitted by the SDK.
    /// Note: To track events, use <code>Analytics.TrackEvent</code>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class AppCenterLog
    {
        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Verbose"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Verbose(string tag, string message, Exception exception)
        {
            Verbose(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Debug"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Debug(string tag, string message, Exception exception)
        {
            Debug(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Info"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Info(string tag, string message, Exception exception)
        {
            Info(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Warn"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Warn(string tag, string message, Exception exception)
        {
            Warn(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Error"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Error(string tag, string message, Exception exception)
        {
            Error(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Assert"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Assert(string tag, string message, Exception exception)
        {
            Assert(tag, ConcatMessageException(message, exception));
        }

        private static string ConcatMessageException(string message, Exception exception)
        {
            return message + "\n" + exception;
        }
    }
}
