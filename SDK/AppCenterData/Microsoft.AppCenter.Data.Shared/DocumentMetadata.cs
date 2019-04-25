// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Document meta-data.
    /// </summary>
    public class DocumentMetadata
    {
        /// <summary>
        /// Cosmos Db document partition.
        /// </summary>
        public string Partition { get; internal set; }

        /// <summary>
        /// Document id.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Document eTag.
        /// </summary>
        public string ETag { get; internal set; }
    }
}
