// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Data service.
    /// </summary>
    public partial class Data
    {
        /// <summary>
        /// Change the URL used to retrieve CosmosDB resource tokens.
        /// </summary>
        /// <param name="tokenExchangeUrl">Token Exchange service URL.</param>
        public static void SetTokenExchangeUrl(string tokenExchangeUrl)
        {
            PlatformSetTokenExchangeUrl(tokenExchangeUrl);
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
        /// <param name="enabled">true to enable, false to disable.</param>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Read the specified partition and documentId.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReadAsync<T>(string partition, string documentId)
        {
            return PlatformReadAsync<T>(partition, documentId, new ReadOptions());
        }

        /// <summary>
        /// Read a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="readOptions">Options for reading and storing the document.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReadAsync<T>(string partition, string documentId, ReadOptions readOptions)
        {
            return PlatformReadAsync<T>(partition, documentId, readOptions);
        }

        /// <summary>
        /// Retrieve a paginated list of the documents in a partition.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.PaginatedDocuments" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<PaginatedDocuments<T>> ListAsync<T>(string partition)
        {
            return PlatformListAsync<T>(partition);
        }

        /// <summary>
        /// Create a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> CreateAsync<T>(string partition, string documentId, T document)
        {
            return PlatformCreateAsync(partition, documentId, document, new WriteOptions());
        }

        /// <summary>
        /// Create a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="writeOptions">Options for writing and storing the document.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> CreateAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return PlatformCreateAsync(partition, documentId, document, writeOptions);
        }

        /// <summary>
        /// Delete a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> DeleteAsync<T>(string partition, string documentId)
        {
            return PlatformDeleteAsync<T>(partition, documentId);
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReplaceAsync<T>(string partition, string documentId, T document)
        {
            return PlatformReplaceAsync(partition, documentId, document, new WriteOptions());
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="writeOptions">Options for writing and storing the document.</param>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReplaceAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return PlatformReplaceAsync(partition, documentId, document, writeOptions);
        }
    }
}
