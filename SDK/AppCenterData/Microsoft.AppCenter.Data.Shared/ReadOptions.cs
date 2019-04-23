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
        public static ReadOptions InfiniteCache { get { return new ReadOptions(TimeSpan.MaxValue); } }

        public static ReadOptions NoCache { get { return new ReadOptions(TimeSpan.Zero); } }

        public ReadOptions()
        {
        }

        public ReadOptions(TimeSpan ttl) : base(ttl)
        {
        }
    }
}