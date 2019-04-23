// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public class Document<T>
    {
        /// <summary>
        /// Deserialized document.
        /// </summary>
        public T Document { get; }

        /// <summary>
        /// Document partition.
        /// </summary>
        public string Partition { get; }

        /// <summary>
        /// Document id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// ETag.
        /// </summary>
        public string ETag { get; }

        /// <summary>
        /// UTC unix timestamp.
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        /// Flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        public bool IsFromCache { get; set; }

        /// <summary>
        /// The pending operation saved in the local storage.
        /// </summary>
        public string PendingOperation { get; set; }

        /// <summary>
        /// Check whether the Data service is enabled or not.
        /// </summary>
        public bool HasFailed
        {
            get
            {
                return DocumentError != null;
            }
        }

        /// <summary>
        /// Document error.
        /// </summary>
        public Exception DocumentError { get; }
    }
}
