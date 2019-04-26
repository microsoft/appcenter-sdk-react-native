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
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.ReadOptions"/> class.
        /// </summary>
        public ReadOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.ReadOptions"/> class.
        /// </summary>
        /// <param name="deviceTimeToLive">Device time to live.</param>
        public ReadOptions(TimeSpan deviceTimeToLive) : base(deviceTimeToLive)
        {
        }
    }
}
