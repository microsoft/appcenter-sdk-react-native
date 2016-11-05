namespace Microsoft.Azure.Mobile
{
    using AndroidMobileCenterLog = Com.Microsoft.Azure.Mobile.Utils.MobileCenterLog;

    public static partial class MobileCenterLog
    {
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
