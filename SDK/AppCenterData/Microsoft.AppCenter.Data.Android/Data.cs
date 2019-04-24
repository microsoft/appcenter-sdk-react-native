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
            return Data.PlatformRead(partition, documentId, new ReadOptions());
        }

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var future = AndroidData.Read(partition, documentId, null /* TODO */);

            return Task.Run(() =>
            {
                var document = (DocumentWrapper)future.Get();

                return new DocumentWrapper<T>()
                {
                    Partition = document.Partition,
                    DeserializedValue = document.DeserializedValue,
                    Id = document.Id,
                    ETag = document.ETag,
                    LastUpdatedDate = document.LastUpdatedDate,
                    FromDeviceCache = document.FromDeviceCache,
                    PendingOperation = document.PendingOperation,
                    Error = new DataException()
                };
            });
        }

        private static Task<PaginatedDocuments<T>> PlatformList<T>(string partition)
        {
            var future = AndroidData.List(partition);

            return Task.Run(() =>
            {
                var doc = (DocumentWrapper)future.Get();

                return new DocumentWrapper<T>()
                {
                    Partition = doc.Partition,
                    DeserializedValue = doc.DeserializedValue,
                    Id = doc.Id,
                    ETag = doc.ETag,
                    LastUpdatedDate = doc.LastUpdatedDate,
                    FromDeviceCache = doc.FromDeviceCache,
                    PendingOperation = doc.PendingOperation,
                    Error = new DataException()
                };
            });
            /*
             var future = AndroidData.Create(partition, documentId, document, writeOptions, null);
            PaginatedDocuments<T> pages = new PaginatedDocuments<T>();
            return Task.Run(() =>
            {
                var page = (Page)future.Get();
                do
                {
                    foreach (var item in page.Items)
                    {
                        var doc = (DocumentWrapper)future.Get();
                        pages.Add(new DocumentWrapper<T>()
                        {
                            Partition = doc.Partition,
                            DeserializedValue = doc.DeserializedValue,
                            Id = doc.Id,
                            ETag = doc.ETag,
                            LastUpdatedDate = doc.LastUpdatedDate,
                            FromDeviceCache = doc.FromDeviceCache,
                            PendingOperation = doc.PendingOperation,
                            Error = new DataException()
                        });
                    }

                } while (page == null);
                return pages;
            });*/
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document)
        {
            return Data.PlatformCreate(partition, documentId, document, new WriteOptions());
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var future = AndroidData.Create(partition, documentId, document, writeOptions, null);

            return Task.Run(() =>
            {
                var doc = (DocumentWrapper)future.Get();

                return new DocumentWrapper<T>()
                {
                    Partition = doc.Partition,
                    DeserializedValue = doc.DeserializedValue,
                    Id = doc.Id,
                    ETag = doc.ETag,
                    LastUpdatedDate = doc.LastUpdatedDate,
                    FromDeviceCache = doc.FromDeviceCache,
                    PendingOperation = doc.PendingOperation,
                    Error = new DataException()
                };
            });
        }

        private static Task<DocumentWrapper<object>> PlatformDelete(string partition, string documentId)
        {
            return Task.FromResult<DocumentWrapper<object>>(null);
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document)
        {
            return Data.PlatformReplace(partition, documentId, document, new WriteOptions());
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var future = AndroidData.Replace(partition, documentId, document, writeOptions, null);

            return Task.Run(() =>
            {
                var doc = (DocumentWrapper)future.Get();

                return new DocumentWrapper<T>()
                {
                    Partition = doc.Partition,
                    DeserializedValue = doc.DeserializedValue,
                    Id = doc.Id,
                    ETag = doc.ETag,
                    LastUpdatedDate = doc.LastUpdatedDate,
                    FromDeviceCache = doc.FromDeviceCache,
                    PendingOperation = doc.PendingOperation,
                    Error = new DataException()
                };
            });
        }
    }
}
