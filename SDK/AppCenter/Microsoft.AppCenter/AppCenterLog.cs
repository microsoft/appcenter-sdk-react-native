// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Microsoft.AppCenter
{
    public static partial class AppCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string LogTag { get; private set; }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Verbose"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Verbose(string tag, string message)
        {
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Debug"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Debug(string tag, string message)
        {
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Info"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Info(string tag, string message)
        {
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Warn"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Warn(string tag, string message)
        {
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Error"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Error(string tag, string message)
        {
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Assert"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Assert(string tag, string message)
        {
        }
    }
}
