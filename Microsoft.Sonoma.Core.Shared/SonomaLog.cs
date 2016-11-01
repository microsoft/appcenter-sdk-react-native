using System;

namespace Microsoft.Sonoma.Core
{
    public static partial class SonomaLog
    {
        public const string LogTag = "SonomaXamarin";

        public static void Verbose(string tag, string message, Exception exception)
        {
            Verbose(tag, ConcatMessageException(message, exception));
        }

        public static void Debug(string tag, string message, Exception exception)
        {
            Debug(tag, ConcatMessageException(message, exception));
        }

        public static void Info(string tag, string message, Exception exception)
        {
            Info(tag, ConcatMessageException(message, exception));
        }

        public static void Warn(string tag, string message, Exception exception)
        {
            Warn(tag, ConcatMessageException(message, exception));
        }

        public static void Error(string tag, string message, Exception exception)
        {
            Error(tag, ConcatMessageException(message, exception));
        }

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
