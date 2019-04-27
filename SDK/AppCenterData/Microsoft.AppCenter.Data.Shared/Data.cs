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
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReadAsync<T>(string documentId, string partition)
        {
            return PlatformReadAsync<T>(documentId, partition, new ReadOptions());
        }

        /// <summary>
        /// Read a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="readOptions">Options for reading and storing the document.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReadAsync<T>(string documentId, string partition, ReadOptions readOptions)
        {
            return PlatformReadAsync<T>(documentId, partition, readOptions);
        }

        /// <summary>
        /// Retrieve a paginated list of the documents in a partition.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.PaginatedDocuments" />.</returns>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <typeparam name="T">The document type.</typeparam>
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
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> CreateAsync<T>(string documentId, T document, string partition)
        {
            return PlatformCreateAsync(documentId, document, partition, new WriteOptions());
        }

        /// <summary>
        /// Create a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="writeOptions">Options for writing and storing the document.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> CreateAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            return PlatformCreateAsync(documentId, document, partition, writeOptions);
        }

        /// <summary>
        /// Delete a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> DeleteAsync<T>(string documentId, string partition)
        {
            return PlatformDeleteAsync<T>(documentId, partition, new WriteOptions());
        }

        /// <summary>
        /// Delete a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="writeOptions">Options for deleting the document.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> DeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            return PlatformDeleteAsync<T>(documentId, partition, writeOptions);
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReplaceAsync<T>(string documentId, T document, string partition)
        {
            return PlatformReplaceAsync(documentId, document, partition, new WriteOptions());
        }

        /// <summary>
        /// Replace a document.
        /// </summary>
        /// <returns>A task with <see cref="T:Microsoft.AppCenter.Data.DocumentWrapper" />.</returns>
        /// <param name="documentId">The CosmosDB document id.</param>
        /// <param name="document">The document to be stored in CosmosDB.</param>
        /// <param name="partition">The CosmosDB partition key.</param>
        /// <param name="writeOptions">Options for writing and storing the document.</param>
        /// <typeparam name="T">The document type.</typeparam>
        /// <exception cref="Microsoft.AppCenter.Data.DataException">If operation failed.</exception>
        public static Task<DocumentWrapper<T>> ReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            return PlatformReplaceAsync(documentId, document, partition, writeOptions);
        }

        /// <summary>
        /// Event handler to suscribe to the remote operations executed as part of offline data synchronization when the network becomes available.
        /// </summary>
        public static event EventHandler<RemoteOperationCompletedEventArgs> RemoteOperationCompleted;
    }
}
