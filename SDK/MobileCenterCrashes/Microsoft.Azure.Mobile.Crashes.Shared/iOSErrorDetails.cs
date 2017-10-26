namespace Microsoft.AAppCenterCrashes
{    
    /// <summary>
    /// Error report details pertinent only to devices running iOS.
    /// </summary>
    public class iOSErrorDetails
    {
        internal iOSErrorDetails(string reporterKey, string signal, string exceptionName, string exceptionReason,
                               uint appProcessIdentifier)
        {
            ReporterKey = reporterKey;
            Signal = signal;
            ExceptionName = exceptionName;
            ExceptionReason = exceptionReason;
            AppProcessIdentifier = appProcessIdentifier;
        }

        /// <summary>
        /// Gets the reporter key.
        /// </summary>
        /// <value>UUID for the app installation on the device.</value>
        public string ReporterKey { get; }

        /// <summary>
        /// Gets the signal that caused the crash.
        /// </summary>
        /// <value>The signal that caused the crash.</value>
        public string Signal { get; }

        /// <summary>
        /// Gets the name of the exception that triggered the crash, <c>null</c> if the crash was not triggered by an
        /// exception.
        /// </summary>
        /// <value>The name of the exception that triggered the crash.</value>
        public string ExceptionName { get; }

        /// <summary>
        /// Gets the reason for the exception that triggered the crash, <c>null</c> if the crash was not triggered by an
        /// exception.
        /// </summary>
        /// <value>The reason for the exception causing the crash.</value>
        public string ExceptionReason { get; }

        /// <summary>
        /// Gets the identifier of the app process that crashed.
        /// </summary>
        /// <value>The app process identifier.</value>
        public uint AppProcessIdentifier { get; }
    }
}
