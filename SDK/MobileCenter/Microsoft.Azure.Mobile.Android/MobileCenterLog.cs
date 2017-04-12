namespace Microsoft.Azure.Mobile
{
    using AndroidMobileCenterLog = Com.Microsoft.Azure.Mobile.Utils.MobileCenterLog;

    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        static MobileCenterLog()
        {
            LogTag = "MobileCenterXamarin";
        }

        public static void Verbose(string tag, string message)
        {
            AndroidMobileCenterLog.Verbose(tag, message);
        }

        public static void Debug(string tag, string message)
        {
            AndroidMobileCenterLog.Debug(tag, message);
        }

        public static void Info(string tag, string message)
        {
            AndroidMobileCenterLog.Info(tag, message);
        }

        public static void Warn(string tag, string message)
        {
            AndroidMobileCenterLog.Warn(tag, message);
        }

        public static void Error(string tag, string message)
        {
            AndroidMobileCenterLog.Error(tag, message);
        }

        public static void Assert(string tag, string message)
        {
            AndroidMobileCenterLog.LogAssert(tag, message);
        }
    }
}
