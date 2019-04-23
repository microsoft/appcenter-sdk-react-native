// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    public abstract class BaseOptions
    {
        /// <summary>
        /// Default caching value of one day.
        /// </summary>
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromDays(1);

        /// <summary>
        /// Device document time-to-live in seconds. Default is one day.
        /// </summary>
        public TimeSpan DeviceTimeToLive { get; }

        protected BaseOptions() : this(DefaultExpiration)
        {
        }

        protected BaseOptions(TimeSpan ttl)
        {
            DeviceTimeToLive = ttl;
        }
    }
}