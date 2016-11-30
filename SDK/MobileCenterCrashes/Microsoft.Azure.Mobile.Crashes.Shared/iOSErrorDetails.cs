using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class iOSErrorDetails
    {
        public iOSErrorDetails(string reporterKey, string signal, string exceptionName, string exceptionReason,
                               uint appProcessIdentifier)
        {
            ReporterKey = reporterKey;
            Signal = signal;
            ExceptionName = exceptionName;
            ExceptionReason = exceptionReason;
            AppProcessIdentifier = appProcessIdentifier;
        }

        public string ReporterKey { get; }
        public string Signal { get; }
        public string ExceptionName { get; }
        public string ExceptionReason { get; }
        public uint AppProcessIdentifier { get; }
    }
}
