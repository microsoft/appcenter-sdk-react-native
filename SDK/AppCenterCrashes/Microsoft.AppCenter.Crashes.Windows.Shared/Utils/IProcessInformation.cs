// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes.Utils
{
    /// <summary>
    /// Utility to get information about the current process. Has different implementations on different platforms. Some information will be unavailable on certain platforms.
    /// </summary>
    public interface IProcessInformation
    {
        /// <summary>
        /// Gets the start time of the current process.
        /// </summary>
        DateTime? ProcessStartTime { get; }
        
        /// <summary>
        /// Gets the ID of the current process.
        /// </summary>
        int? ProcessId { get; }

        /// <summary>
        /// Gets the name of the current process.
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Gets the ID of the parent process.
        /// </summary>
        int? ParentProcessId { get; }
      
        /// <summary>
        /// Gets the name of the parent process.
        /// </summary>
        string ParentProcessName { get; }

        /// <summary>
        /// Gets the CPU architecture that the current process is running on.
        /// </summary>
        string ProcessArchitecture { get; }
    }
}
