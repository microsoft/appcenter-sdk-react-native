// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
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
        /// SDK emits debug, info, warn, error and assert logs.
        /// </summary>
        Debug,

        /// <summary>
        /// SDK emits info, warn, error, and assert logs.
        /// </summary>
        Info,

        /// <summary>
        /// SDK emits warn, error, and assert logs.
        /// </summary>
        Warn,

        /// <summary>
        /// SDK error and assert logs.
        /// </summary>
        Error,

        /// <summary>
        /// Only assert logs are emitted by SDK.
        /// </summary>
        Assert,

        /// <summary>
        /// No log is emitted by SDK.
        /// </summary>
        None
    }
}
