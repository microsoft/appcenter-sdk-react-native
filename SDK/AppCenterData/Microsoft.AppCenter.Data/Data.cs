// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        private static void PlatformSetTokenExchangeUrl(string tokenExchangeUrl)
        {
        }

        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        private static Task<DocumentWrapper<T>> PlatformReadAsync<T>(string partition, string documentId, ReadOptions readOptions)
        {
            return GenerateFailedTask<DocumentWrapper<T>>();
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            return GenerateFailedTask<PaginatedDocuments<T>>();
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return GenerateFailedTask<DocumentWrapper<T>>();
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string partition, string documentId)
        {
            return GenerateFailedTask<DocumentWrapper<T>>();
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return GenerateFailedTask<DocumentWrapper<T>>();
        }

        private static Task<T> GenerateFailedTask<T>()
        {
            var task = new TaskCompletionSource<T>();
            task.SetException(new DataException(null, new NotImplementedException()));
            return task.Task;
        }
    }
}