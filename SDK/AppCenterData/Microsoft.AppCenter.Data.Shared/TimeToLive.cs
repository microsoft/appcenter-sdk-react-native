// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    public static class TimeToLive
    {
        /// <summary>
        /// Cache does not expire.
        /// </summary>
        public static readonly TimeSpan Infinite = TimeSpan.FromSeconds(-1);

        /// <summary>
        /// Do not cache documents.
        /// </summary>
        public static readonly TimeSpan NoCache = TimeSpan.Zero;

        /// <summary>
        /// Default caching value of one day (in seconds).
        /// </summary>
        public static readonly TimeSpan Default = TimeSpan.FromDays(1);
    }
}
