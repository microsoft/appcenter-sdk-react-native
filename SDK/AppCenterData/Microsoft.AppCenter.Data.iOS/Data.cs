using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Data.iOS.Bindings;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Data;
using ObjCRuntime;

namespace Microsoft.AppCenter.Data
{
    public partial class Data : AppCenterService
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Crashes bindings type.
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

        private static void PlatformSetApiUrl(string apiUrl)
        {
            MSDataStore.SetTokenExchangeUrl(apiUrl);
        }

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();

            MSDataStore.Read(partition, documentId, (resultDoc) => 
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformRead<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();

            MSReadOptions msReadOptions = new MSReadOptions 
            {
                DeviceTimeToLive= readOptions.DeviceTimeToLive.Ticks
            };
            MSDataStore.Read(partition, documentId, msReadOptions, (resultDoc) =>
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<PaginatedDocuments<T>> PlatformList<T>(string partition)
        {
            var taskCompletionSource = new TaskCompletionSource<PaginatedDocuments<T>>();
            PaginatedDocuments<T> paginatedDocs = new PaginatedDocuments<T>();
            MSDataStore.List(partition, (resultPages) =>
            {
                MSPage currentPage = resultPages.CurrentPage() as MSPage;
                do
                {
                    foreach (var item in currentPage.Items)
                    {
                        DocumentWrapper<T> doc = new DocumentWrapper<T>
                        {
                            Partition = item.Partition,
                            Id = item.DocumentId,
                            DeserializedValue = Document<T>.DeserializeString(item.DeserializedValue),
                            ETag = item.ETag,
                            LastUpdatedDate = ((DateTime)item.LastUpdatedDate).Ticks,
                            IsFromDeviceCache = item.FromDeviceCache,
                            Error = new DataException(item.Error.Error.LocalizedDescription)
                        };

                        paginatedDocs.Add(doc);
                    }

                    resultPages.NextPage((page) =>
                    {
                        currentPage = page as MSPage;
                    });
                }while (currentPage == null);

                taskCompletionSource.TrySetResult(paginatedDocs);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();

            MSDataStore.Create(partition, documentId, document.ToString(), (resultDoc) =>
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformCreate<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();

            MSWriteOptions msWriteOptions = new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };

            MSDataStore.Create(partition, documentId, document.ToString(), msWriteOptions, (resultDoc) =>
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            MSDataStore.Replace(partition, documentId, document.ToString(), (resultDoc) =>
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();

            MSWriteOptions msWriteOptions = new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };

            MSDataStore.Replace(partition, documentId, document.ToString(), msWriteOptions, (resultDoc) =>
            {
                DocumentWrapper<T> doc = new DocumentWrapper<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DeserializedValue = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<object>> PlatformDelete(string partition, string documentId)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<object>>();
            MSDataStore.Delete(partition, documentId, (resultDoc) =>
            {
                DocumentWrapper<object> doc = new DocumentWrapper<object>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    ETag = resultDoc.ETag,
                    LastUpdatedDate = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromDeviceCache = resultDoc.FromDeviceCache,
                    Error = new DataException(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }
    }
}
