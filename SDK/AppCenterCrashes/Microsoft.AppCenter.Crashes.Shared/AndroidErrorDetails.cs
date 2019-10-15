// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Error report details pertinent only to devices running Android.
    /// </summary>
    public class AndroidErrorDetails
    {
        internal AndroidErrorDetails(string stackTrace, string threadName)
        {
            StackTrace  = stackTrace;
            ThreadName = threadName;
        }

        /// <summary>
        /// Gets the throwable associated with the Java crash.
        /// </summary>
        /// <value>The throwable associated with the crash. <c>null</c> if the crash occured in .NET code.</value>
        [Obsolete("This property is no longer set due to a security issue, use StackTrace instead.")]
        public object Throwable { get; }

        /// <summary>
        /// Gets the stack trace associated with the Java crash.
        /// </summary>
        /// <value>The stack trace associated with the crash. <c>null</c> if the crash occured in .NET code.</value>
        public string StackTrace { get; }

        /// <summary>
        /// Gets the name of the thread that crashed.
        /// </summary>
        /// <value>The name of the thread that crashed.</value>
        public string ThreadName { get; }
    }
}
