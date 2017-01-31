namespace Microsoft.Azure.Mobile
{
    public static partial class MobileCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        static MobileCenterLog()
        {
            LogTag = "MobileCenter";
        }

        public static void Verbose(string tag, string message)
        {
            //TODO implement me
        }

        public static void Debug(string tag, string message)
        {
            //TODO implement me
        }

        public static void Info(string tag, string message)
        {
            //TODO implement me
        }

        public static void Warn(string tag, string message)
        {
            //TODO implement me
        }

        public static void Error(string tag, string message)
        {
            //TODO implement me
        }

        public static void Assert(string tag, string message)
        {
            //TODO implement me
        }
    }
}
