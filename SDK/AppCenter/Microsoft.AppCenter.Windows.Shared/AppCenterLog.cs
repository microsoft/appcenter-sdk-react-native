#define DEBUG
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.AppCenter
{
    using System;
    using System.Globalization;

    public static partial class AppCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string LogTag { get; private set; }
        private static readonly object LogLock = new object();
        private static LogLevel _level = LogLevel.Assert;

        /// <summary>
        /// Gets or sets the log verbosity level.
        /// </summary>
        internal static LogLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                lock (LogLock)
                {
                    _level = value;
                }
            }
        }
        static AppCenterLog()
        {
            LogTag = "AppCenter";
            if (Debugger.IsAttached)
            {
                _level = LogLevel.Warn;
            }
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Verbose"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Verbose(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Verbose, "VERBOSE");
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Debug"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Debug(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Debug, "DEBUG");
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Info"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Info(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Info, "INFO");
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Warn"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Warn(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Warn, "WARN");
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Error"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Error(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Error, "ERROR");
        }

        /// <summary>
        /// Writes a log at the <see cref="LogLevel.Assert"/> level.
        /// Note: To track events, use <code>Analytics.TrackEvent</code>.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Assert(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Assert, "ASSERT");
        }

        private static void LogMessage(string tag, string message, LogLevel level, string levelName)
        {
            lock (LogLock)
            {
                if (Level <= level)
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    System.Diagnostics.Debug.WriteLine($"{timestamp} [{tag}] {levelName}: {message}");
                }
            }
        }
    }
}
