namespace Microsoft.Sonoma.Core
{
    using AndroidSonomaLog = Com.Microsoft.Sonoma.Core.Utils.SonomaLog;

    public static partial class SonomaLog
    {
        public static void Verbose(string tag, string message)
        {
            AndroidSonomaLog.Verbose(tag, message);
        }

        public static void Debug(string tag, string message)
        {
            AndroidSonomaLog.Debug(tag, message);
        }

        public static void Error(string tag, string message)
        {
            AndroidSonomaLog.Error(tag, message);
        }

        public static void Info(string tag, string message)
        {
            AndroidSonomaLog.Info(tag, message);
        }

        public static void Warn(string tag, string message)
        {
            AndroidSonomaLog.Warn(tag, message);
        }
    }
}
