using System;

namespace Microsoft.Azure.Mobile
{
    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the Xamarin SDK level will contain this tag.
        /// </summary>
        public const string LogTag = "MobileCenterXamarin";

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Verbose"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        public static void Verbose(string tag, string message, Exception exception)
        {
            Verbose(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Debug"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        public static void Debug(string tag, string message, Exception exception)
        {
            Debug(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Info"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        public static void Info(string tag, string message, Exception exception)
        {
            Info(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Warn"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        public static void Warn(string tag, string message, Exception exception)
        {
            Warn(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Error"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
        public static void Error(string tag, string message, Exception exception)
        {
            Error(tag, ConcatMessageException(message, exception));
        }

        /// <summary>
        /// Writes a log and an exception at the <see cref="LogLevel.Assert"/> level. 
        /// </summary>
        /// <param name="tag">Log tag.</param>
        /// <param name="message">Message.</param>
        /// <param name="exception">Associated exception.</param>
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
