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

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msReadOptions = ConvertReadOptionsToInternal(readOptions);
            MSDataStore.Read(partition, documentId, msReadOptions, (resultDoc) =>
            {
                taskCompletionSource.TrySetResult(ConvertInternalDocToExternal<T>(resultDoc));
            });
            return taskCompletionSource.Task;
        }

        private static Task<PaginatedDocuments<T>> PlatformList<T>(string partition)
        {
            var taskCompletionSource = new TaskCompletionSource<PaginatedDocuments<T>>();
            MSDataStore.List(partition, (resultPages) =>
            {
                var paginatedDocs = new PaginatedDocuments<T>(resultPages);
                taskCompletionSource.TrySetResult(paginatedDocs);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = ConvertWriteOptionsToInternal(writeOptions);
            MSDataStore.Create(partition, documentId, document.ToString(), msWriteOptions, (resultDoc) =>
            {
                taskCompletionSource.TrySetResult(ConvertInternalDocToExternal<T>(resultDoc));
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = ConvertWriteOptionsToInternal(writeOptions);
            MSDataStore.Replace(partition, documentId, JsonConvert.SerializeObject(document), msWriteOptions, (resultDoc) =>
            {
                taskCompletionSource.TrySetResult(ConvertInternalDocToExternal<T>(resultDoc));
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformDelete<T>(string partition, string documentId)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            MSDataStore.Delete(partition, documentId, (resultDoc) =>
            {
                taskCompletionSource.TrySetResult(ConvertInternalDocToExternal<T>(resultDoc));
            });
            return taskCompletionSource.Task;
        }

        internal static DocumentWrapper<T> ConvertInternalDocToExternal<T>(MSDocumentWrapper internalDoc) 
        {
            return new DocumentWrapper<T>
            {
                Partition = internalDoc.Partition,
                Id = internalDoc.DocumentId,
                DeserializedValue = JsonConvert.DeserializeObject<T>(internalDoc.JsonValue),
                ETag = internalDoc.ETag,
                LastUpdatedDate = (DateTime)internalDoc.LastUpdatedDate,
                FromDeviceCache = internalDoc.FromDeviceCache,
                Error = ConvertErrorToException(internalDoc.Error)
            };
        }

        internal static DataException ConvertErrorToException(MSDataSourceError error) 
        {
            var exception = new NSErrorException(error.Error);
            return new DataException(exception.Message, exception);
        }

        private static MSWriteOptions ConvertWriteOptionsToInternal(WriteOptions writeOptions)
        {
            return new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };
        }

        private static MSReadOptions ConvertReadOptionsToInternal(ReadOptions readOptions)
        {
            return new MSReadOptions
            {
                DeviceTimeToLive = readOptions.DeviceTimeToLive.Ticks
            };
        }
    }
}
