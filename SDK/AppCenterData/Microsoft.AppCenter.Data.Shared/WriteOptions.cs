// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Write options.
    /// </summary>
    public class WriteOptions : BaseOptions
    {
        /// <summary>
        /// Cache does not expire.
        /// </summary>
        public static WriteOptions InfiniteCache { get => new WriteOptions(TimeSpan.MaxValue); }

        /// <summary>
        /// Do not cache documents locally.
        /// </summary>
        public static WriteOptions NoCache { get => new WriteOptions(TimeSpan.Zero); }

        public WriteOptions()
        {
        }

        public WriteOptions(TimeSpan ttl) : base(ttl)
        {
        }
    }
}