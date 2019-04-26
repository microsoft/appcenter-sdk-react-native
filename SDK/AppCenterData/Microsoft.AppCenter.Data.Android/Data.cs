// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Data;
using Com.Microsoft.Appcenter.Data.Exception;
using Com.Microsoft.Appcenter.Data.Models;
using Com.Microsoft.Appcenter.Utils.Async;
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

        static Data()
        {
            // Set up bridge between Java listener and .NET events/callbacks.
            AndroidData.SetDataStoreRemoteOperationListener(new AndroidDataEventListener());
        }

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
            return RunDocumentWrapperTask<T>(future);
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
            return RunJsonTask(() =>
            {
                var future = AndroidData.Create(documentId, ToJsonElement(document), JsonElementClass, partition, writeOptions.ToAndroidWriteOptions());
                return ToDocumentWrapper<T>(future);
            });
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            var future = AndroidData.Delete(documentId, partition, writeOptions.ToAndroidWriteOptions());
            return RunDocumentWrapperTask<T>(future);
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            return RunJsonTask(() =>
            {
                var future = AndroidData.Replace(documentId, ToJsonElement(document), JsonElementClass, partition, writeOptions.ToAndroidWriteOptions());
                return ToDocumentWrapper<T>(future);
            });
        }

        private static Task<DocumentWrapper<T>> RunDocumentWrapperTask<T>(IAppCenterFuture future)
        {
            return RunJsonTask(() =>
            {
                return ToDocumentWrapper<T>(future);
            });
        }

        private static DocumentWrapper<T> ToDocumentWrapper<T>(IAppCenterFuture future)
        {
            var documentWrapper = (AndroidDocumentWrapper)future.Get();
            return documentWrapper.ToDocumentWrapper<T>();
        }

        private static Task<T> RunJsonTask<T>(Func<T> backgroundCode)
        {
            // We run JSON inside a task, see https://forums.xamarin.com/discussion/94867/how-to-speed-up-newtonsoft-json-on-android
            return Task.Run(() =>
            {
                try
                {
                    return backgroundCode();
                }
                catch (JsonException e)
                {
                    throw new DataException("Document parsing failed.", e);
                }
            });
        }

        private static JsonElement ToJsonElement<T>(T document)
        {
            var jsonValue = JsonConvert.SerializeObject(document);
            return (JsonElement)Gson.FromJson(jsonValue, JsonElementClass);
        }

        /// <summary>
        /// Bridge between C# events/callbacks and Java listeners.
        /// </summary>
        class AndroidDataEventListener : Java.Lang.Object, IDataStoreEventListener
        {
            public void OnDataStoreOperationResult(string operation, AndroidDocumentMetadata document, AndroidDataException error)
            {
                if (RemoteOperationCompleted == null)
                {
                    return;
                }
                var documentMetadata = document.ToDocumentMetadata();
                var eventArgs = new RemoteOperationCompletedEventArgs
                {
                    Operation = operation,
                    DocumentMetadata = documentMetadata,
                    Error = error?.ToDataException()
                };
                RemoteOperationCompleted(documentMetadata, eventArgs);
            }
        }
    }
}
