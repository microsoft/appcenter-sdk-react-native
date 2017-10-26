namespace Microsoft.AAppCenter
{
    using AndroidAppCenterLog = Com.Microsoft.AppCenter.Utils.AppCenterLog;

    public static partial class AppCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        static AppCenterLog()
        {
            LogTag = "AppCenterXamarin";
        }

        public static void Verbose(string tag, string message)
        {
            AndroidAppCenterLog.Verbose(tag, message);
        }

        public static void Debug(string tag, string message)
        {
            AndroidAppCenterLog.Debug(tag, message);
        }

        public static void Info(string tag, string message)
        {
            AndroidAppCenterLog.Info(tag, message);
        }

        public static void Warn(string tag, string message)
        {
            AndroidAppCenterLog.Warn(tag, message);
        }

        public static void Error(string tag, string message)
        {
            AndroidAppCenterLog.Error(tag, message);
        }

        public static void Assert(string tag, string message)
        {
            AndroidAppCenterLog.LogAssert(tag, message);
        }
    }
}
