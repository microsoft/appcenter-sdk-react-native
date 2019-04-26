// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Base class for read and write options.
    /// </summary>
    public abstract class BaseOptions
    {
        /// <summary>
        /// Device document time-to-live. Default is one day.
        /// </summary>
        public TimeSpan DeviceTimeToLive { get; }

        internal BaseOptions() : this(TimeToLive.Default)
        {
        }

        internal BaseOptions(TimeSpan deviceTimeToLive)
        {
            DeviceTimeToLive = deviceTimeToLive;
        }
    }
}