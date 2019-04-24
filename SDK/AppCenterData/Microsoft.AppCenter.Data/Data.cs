// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            return Task.FromResult<PaginatedDocuments<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }
    }
}