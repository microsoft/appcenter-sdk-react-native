// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Read options.
    /// </summary>
    public class ReadOptions : BaseOptions
    {
        /// <summary>
        /// Cache does not expire.
        /// </summary>
        public static ReadOptions InfiniteCache { get => new ReadOptions(TimeSpan.MaxValue); }

        /// <summary>
        /// Do not cache documents locally.
        /// </summary>
        public static ReadOptions NoCache { get => new ReadOptions(TimeSpan.Zero); }

        public ReadOptions()
        {
        }

        public ReadOptions(TimeSpan ttl) : base(ttl)
        {
        }
    }
}