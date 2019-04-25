// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// A document coming back from CosmosDB.
    /// </summary>
    public class DocumentWrapper<T> : DocumentMetadata
    {
        /// <summary>
        /// Deserialized document.
        /// </summary>
        public T DeserializedValue { get; internal set; }

        /// <summary>
        /// Non-serialized document.
        /// </summary>
        public string JsonValue { get; internal set; }

        /// <summary>
        /// Last update timestamp.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; internal set; }

        /// <summary>
        /// Flag indicating if data was retrieved from the local cache.
        /// </summary>
        public bool IsFromDeviceCache { get; internal set; }
    }
}
