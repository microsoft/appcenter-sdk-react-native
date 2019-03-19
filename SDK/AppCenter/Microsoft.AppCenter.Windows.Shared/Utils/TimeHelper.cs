// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Utility for getting time information
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// Gets the current time in milliseconds
        /// </summary>
        /// <returns>The current time in milliseconds</returns>
        public static long CurrentTimeInMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
