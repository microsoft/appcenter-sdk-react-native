using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class iOSErrorDetails
    {
        public iOSErrorDetails(string _ReporterKey, string _Signal, string _ExceptionName, string _ExceptionReason,
                               uint _AppProcessIdentifier)
        {
            ReporterKey = _ReporterKey;
            Signal = _Signal;
            ExceptionName = _ExceptionName;
            ExceptionReason = _ExceptionReason;
            AppProcessIdentifier = _AppProcessIdentifier;
        }

        public string ReporterKey { get; }
        public string Signal { get; }
        public string ExceptionName { get; }
        public string ExceptionReason { get; }
        public uint AppProcessIdentifier { get; }
    }
}
