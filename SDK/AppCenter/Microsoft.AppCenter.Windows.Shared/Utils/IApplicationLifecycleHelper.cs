// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Represents an object that tracks the application lifecycle.
    /// </summary>
    public interface IApplicationLifecycleHelper
    {
        /// <summary>
        /// Indicates whether the application is currently in a suspended state
        /// </summary>
        bool IsSuspended { get; }

        /// <summary>
        /// Occurs when the application has just been suspended
        /// </summary>
        event EventHandler ApplicationSuspended;

        /// <summary>
        /// Occurs when the application is about to resume.
        /// Note that in UWP, this corresponds to CoreApplication.LeavingBackground for builds 14393 and up,
        /// but to CoreApplication.Resuming for builds under 14393.
        /// </summary>
        event EventHandler ApplicationResuming;
    
        /// <summary>
        /// Occurs when an unhandled exception is fired
        /// </summary>
        /// <remarks>This is used to set up the shutdown logic in the event of a crash.</remarks>
        event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
