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
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.WriteOptions"/> class.
        /// </summary>
        public WriteOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AppCenter.Data.WriteOptions"/> class.
        /// </summary>
        /// <param name="deviceTimeToLive">Device time to live.</param>
        public WriteOptions(TimeSpan deviceTimeToLive) : base(deviceTimeToLive)
        {
        }
    }
}
