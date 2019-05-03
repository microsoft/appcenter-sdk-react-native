// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils.Synchronization
{
    /// <summary>
    /// Exception thrown when a StatefulMutex cannot acquire a lock
    /// </summary>
    /// <seealso cref="StatefulMutex"/> 
    public class StatefulMutexException : AppCenterException
    {
        public StatefulMutexException(string message) : base(message)
        {
        }

        public StatefulMutexException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
