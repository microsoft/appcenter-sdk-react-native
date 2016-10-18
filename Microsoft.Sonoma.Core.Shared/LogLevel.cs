namespace Microsoft.Sonoma.Core
{
    /// <summary>
    /// Log level threshold for logs emitted by the SDK.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// SDK emits all possible level of logs.
        /// </summary>
        Verbose,

        /// <summary>
        /// SDK emits debug, info, warn, and error logs.
        /// </summary>
        Debug,

        /// <summary>
        /// SDK emits info, warn, and error logs.
        /// </summary>
        Info,

        /// <summary>
        /// Only error and warn logs are emitted by the SDK.
        /// </summary>
        Warn,

        /// <summary>
        /// Only error logs are emitted by the SDK.
        /// </summary>
        Error,

        /// <summary>
        /// No log is emitted by the SDK.
        /// </summary>
        Assert
    }
}
