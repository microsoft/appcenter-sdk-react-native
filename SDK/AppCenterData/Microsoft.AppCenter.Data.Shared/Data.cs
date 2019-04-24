// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        /// <summary>
        /// Change the base URL used to make API calls.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        public static void SetApiUrl(string apiUrl)
        {
            PlatformSetApiUrl(apiUrl);
        }

        /// <summary>
        /// Check whether the Data service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        /// Enable or disable the Data service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Read a document.
        /// </summary>
        public static Task<Document<T>> Read<T>(string partition, string documentId)
        {
            return PlatformRead<T>(partition, documentId);
        }

        /// <summary>
        /// Read a document.
        /// </summary>
        public static Task<Document<T>> Read<T>(string partition, string documentId, ReadOptions readOptions)
        {
            return PlatformRead<T>(partition, documentId, readOptions);
        }

        /// <summary>
        /// List (need optional signature to configure page size).
        /// </summary>
        public static Task<PaginatedDocuments<T>> List<T>(string partition)
        {
            return PlatformList<T>(partition);
        }

        /// <summary>
        /// Create a document.
        /// </summary>
        public static Task<Document<T>> Create<T>(string partition, string documentId, T document)
        {
            return PlatformCreate(partition, documentId, document);
        }

        /// <summary>
        /// Create a document.
        /// </summary>
        public static Task<Document<T>> Create<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return PlatformCreate(partition, documentId, document, writeOptions);
        }

        /// <summary>
        /// Delete a document.
        /// </summary>
        public static Task<Document<T>> Delete<T>(string partition, string documentId)
        {
            return PlatformDelete<T>(partition, documentId);
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        public static Task<Document<T>> Replace<T>(string partition, string documentId, T document)
        {
            return PlatformReplace(partition, documentId, document);
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        public static Task<Document<T>> Replace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return PlatformReplace(partition, documentId, document, writeOptions);
        }
    }
}
