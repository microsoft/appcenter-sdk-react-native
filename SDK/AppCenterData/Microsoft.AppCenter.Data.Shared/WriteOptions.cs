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
        public static WriteOptions InfiniteCache { get { return new WriteOptions(TimeSpan.MaxValue); } }

        public static WriteOptions NoCache { get { return new WriteOptions(TimeSpan.Zero); } }

        public WriteOptions()
        {
        }

        public WriteOptions(TimeSpan ttl) : base(ttl)
        {
        }
    }
}