// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AppCenter.Data.iOS.Bindings;

namespace Microsoft.AppCenter.Data
{
    public partial class Data : AppCenterService
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Data bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(MSDataStore);

        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSDataStore.IsEnabled());
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSDataStore.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        private static void PlatformSetTokenExchangeUrl(string tokenExchangeUrl)
        {
            MSDataStore.SetTokenExchangeUrl(tokenExchangeUrl);
        }

        private static Task<DocumentWrapper<T>> PlatformReadAsync<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msReadOptions = readOptions.ToMSReadOptions();
            MSDataStore.Read(partition, documentId, msReadOptions, resultDoc =>
            {
                if (resultDoc.Error != null)
                {
                    taskCompletionSource.TrySetException(new NSErrorException(resultDoc.Error.Error));
                }
                else
                {
                    taskCompletionSource.TrySetResult(resultDoc.ToDocumentWrapper<T>());
                }
            });
            return taskCompletionSource.Task;
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            var taskCompletionSource = new TaskCompletionSource<PaginatedDocuments<T>>();
            MSDataStore.List(partition, resultPages =>
            {
                var paginatedDocs = new PaginatedDocuments<T>(resultPages);
                taskCompletionSource.TrySetResult(paginatedDocs);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            MSDataStore.Create(partition, documentId, JsonConvert.SerializeObject(document), msWriteOptions, resultDoc =>
            {
                if (resultDoc.Error != null)
                {
                    taskCompletionSource.TrySetException(new NSErrorException(resultDoc.Error.Error));
                }
                else
                {
                    taskCompletionSource.TrySetResult(resultDoc.ToDocumentWrapper<T>());
                }
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            MSDataStore.Replace(partition, documentId, JsonConvert.SerializeObject(document), msWriteOptions, resultDoc =>
            {
                if (resultDoc.Error != null)
                {
                    taskCompletionSource.TrySetException(new NSErrorException(resultDoc.Error.Error));
                }
                else
                {
                    taskCompletionSource.TrySetResult(resultDoc.ToDocumentWrapper<T>());
                }
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            MSDataStore.Delete(partition, documentId, resultDoc =>
            {
                if (resultDoc.Error != null)
                {
                    taskCompletionSource.TrySetException(new NSErrorException(resultDoc.Error.Error));
                }
                else
                {
                    taskCompletionSource.TrySetResult(resultDoc.ToDocumentWrapper<T>());
                }
            });
            return taskCompletionSource.Task;
        }
    }
}
