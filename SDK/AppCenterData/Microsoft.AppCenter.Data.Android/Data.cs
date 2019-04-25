// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Data;
using Com.Microsoft.Appcenter.Data.Models;
using GoogleGson;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Data bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidData);

        private static readonly Java.Lang.Class JsonElementClass = Java.Lang.Class.FromType(typeof(JsonElement));

        private static readonly Gson Gson = new GsonBuilder().Create();

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

        private static Task<DocumentWrapper<T>> PlatformReadAsync<T>(string documentId, string partition, ReadOptions readOptions)
        {
            var future = AndroidData.Read(documentId, JsonElementClass, partition, readOptions.ToAndroidReadOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            var future = AndroidData.List(JsonElementClass, partition);
            return Task.Run(() =>
            {
                var paginatedDocuments = (AndroidPaginatedDocuments)future.Get();
                return paginatedDocuments.ToPaginatedDocuments<T>();
            });
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var future = AndroidData.Create(documentId, ToJsonElement(document), JsonElementClass, partition, writeOptions.ToAndroidWriteOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            var future = AndroidData.Delete(documentId, partition, writeOptions.ToAndroidWriteOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var future = AndroidData.Replace(documentId, ToJsonElement(document), JsonElementClass, partition, writeOptions.ToAndroidWriteOptions());
            return Task.Run(() =>
            {
                var documentWrapper = (AndroidDocumentWrapper)future.Get();
                return documentWrapper.ToDocumentWrapper<T>();
            });
        }

        private static JsonElement ToJsonElement<T>(T document)
        {
            var jsonValue = JsonConvert.SerializeObject(document);
            return (JsonElement)Gson.FromJson(jsonValue, JsonElementClass);
        }
    }
}
