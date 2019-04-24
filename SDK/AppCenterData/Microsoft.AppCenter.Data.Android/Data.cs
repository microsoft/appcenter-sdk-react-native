// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Data;
using Com.Microsoft.Appcenter.Data.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Data bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidData);

        private static void PlatformSetTokenExchangeUrl(string tokenExchangeUrl)
        {
            AndroidData.SetTokenExchangeUrl(tokenExchangeUrl);
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

        private static Task<DocumentWrapper<T>> PlatformReadAsync<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var future = AndroidData.Read(partition, documentId, null, readOptions.ToAndroidReadOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            var future = AndroidData.List(partition, null);
            return Task.FromResult<PaginatedDocuments<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var jsonValue = JsonConvert.SerializeObject(document);
            var future = AndroidData.Create(partition, documentId, jsonValue, null, writeOptions.ToAndroidWriteOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<T>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var jsonValue = JsonConvert.SerializeObject(document);
            var future = AndroidData.Replace(partition, documentId, jsonValue, null, writeOptions.ToAndroidWriteOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }
    }
}
