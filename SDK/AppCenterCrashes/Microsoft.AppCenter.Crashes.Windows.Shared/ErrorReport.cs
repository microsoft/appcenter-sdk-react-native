// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Crashes.Ingestion.Models;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Error report containing information about a particular crash.
    /// </summary>
    public partial class ErrorReport
    {
        /// <summary>
        /// Creates a new error report.
        /// </summary>
        /// <param name="log">The managed error log.</param>
        /// <param name="exception">The associated exception object.</param>
        public ErrorReport(ManagedErrorLog log, System.Exception exception)
        {
            Id = log.Id.ToString();
            AppStartTime = new DateTimeOffset(log.AppLaunchTimestamp.Value);
            AppErrorTime = new DateTimeOffset(log.Timestamp.Value);
            Device = new Device(log.Device);
            Exception = exception;
        }
    }
}
