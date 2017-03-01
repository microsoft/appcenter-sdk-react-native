#define DEBUG
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Mobile
{
    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }
        private static readonly object LogLock = new object();
        private static LogLevel _level;

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

            /* 
             * For unknown reasons, but possibly reasons specific to NuGet or UWP,
             * simply defining the DEBUG symbol in this file is not enough to enable 
             * Debug.WriteLine(). The following workaround is adapted from
             * http://stackoverflow.com/a/23669216
             */
            var parameter = Expression.Parameter(typeof(string), "text");
            var callExp = Expression.Call(
                  typeof(System.Diagnostics.Debug).GetRuntimeMethod(
                       "WriteLine", new[] { typeof(string) }),
                  parameter);
            _loggingAction = Expression.Lambda<Action<string>>(callExp, parameter).Compile();
        }

        private static Action<string> _loggingAction;
        public static void Verbose(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Verbose, "VERBOSE");
        }

        public static void Debug(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Debug, "DEBUG");
        }

        public static void Info(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Info, "INFO");
        }

        public static void Warn(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Warn, "WARN");
        }

        public static void Error(string tag, string message)
        {
            LogMessage(tag, message, LogLevel.Error, "ERROR");
        }

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
