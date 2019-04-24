// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// A document coming back from CosmosDB.
    /// </summary>
    public class DocumentWrapper<T>
    {
        /// <summary>
        /// Deserialized document.
        /// </summary>
        public T DeserializedValue { get; internal set; }

        /// <summary>
        /// Document partition.
        /// </summary>
        public string Partition { get; internal set; }

        /// <summary>
        /// Document id.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// ETag.
        /// </summary>
        public string ETag { get; internal set; }

        /// <summary>
        /// UTC unix timestamp.
        /// </summary>
        public long LastUpdatedDate { get; internal set; }

        /// <summary>
        /// Flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        public bool IsFromDeviceCache { get; set; }

        /// <summary>
        /// The pending operation saved in the local storage.
        /// </summary>
        public string PendingOperation { get; set; }

        /// <summary>
        /// Check whether the Data service is enabled or not.
        /// </summary>
        public bool HasFailed { get => Error != null; }

        /// <summary>
        /// Document error.
        /// </summary>
        public DataException Error { get; internal set; }
    }
}
