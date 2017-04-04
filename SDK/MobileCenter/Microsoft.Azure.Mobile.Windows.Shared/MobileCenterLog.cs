#define DEBUG
using System.Diagnostics;

namespace Microsoft.Azure.Mobile
{
    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
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
        static MobileCenterLog()
        {
            LogTag = "MobileCenter";
            if (Debugger.IsAttached)
            {
                _level = LogLevel.Warn;
            }
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Verbose"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        public static void Verbose(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Verbose, "VERBOSE");
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        public static void Debug(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Debug, "DEBUG");
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        public static void Info(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Info, "INFO");
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Warn"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        public static void Warn(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Warn, "WARN");
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Error"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
        public static void Error(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Error, "ERROR");
        }

        /// <summary>
        /// Emits a log at the <see cref="LogLevel.Assert"/> level.
        /// </summary>
        /// <param name="tag">The log tag</param>
        /// <param name="message">The message to log</param>
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
                    System.Diagnostics.Debug.WriteLine($"[{tag}] {levelName}: {message}");
                }
            }
        }
    }
}
