// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Storage;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        [Preserve]
        public static Type BindingType => typeof(AndroidData);

        private static void PlatformSetApiUrl(string apiUrl)
        {
            AndroidData.SetApiUrl(apiUrl);
        }

        private static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidData.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidData.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId)
        {
            var future = AndroidData.Read(partition, documentId, null /* TODO */);
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

        private static Task<DocumentWrapper<object>> PlatformDelete(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<object>>(null);
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
