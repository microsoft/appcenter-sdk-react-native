// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        private static void PlatformSetTokenExchangeUrl(string apiUrl)
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

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId, ReadOptions readOptions)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<PaginatedDocuments<T>> PlatformList<T>(string partition)
        {
            return Task.FromResult<PaginatedDocuments<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformDelete<T>(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }
    }
}