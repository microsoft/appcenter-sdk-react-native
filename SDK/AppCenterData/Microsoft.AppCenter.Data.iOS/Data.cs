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

        private static Task<Document<T>> PlatformRead<T>(string partition, string documentId)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();

            MSDataStore.Read(partition, documentId, (resultDoc) => 
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<Document<T>> PlatformRead<T>(string partition, string documentId, ReadOptions readOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();

            MSReadOptions msReadOptions = new MSReadOptions 
            {
                DeviceTimeToLive= readOptions.DeviceTimeToLive.Ticks
            };
            MSDataStore.Read(partition, documentId, msReadOptions, (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
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
                        Document<T> doc = new Document<T>
                        {
                            Partition = item.Partition,
                            Id = item.DocumentId,
                            DocumentData = Document<T>.DeserializeString(item.DeserializedValue),
                            ETag = item.ETag,
                            Timestamp = ((DateTime)item.LastUpdatedDate).Ticks,
                            IsFromCache = item.FromDeviceCache,
                            DocumentError = new Exception(item.Error.Error.LocalizedDescription)
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

        private static Task<Document<T>> PlatformCreate<T>(string partition, string documentId, T document)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();

            MSDataStore.Create(partition, documentId, document.ToString(), (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<Document<T>> PlatformCreate<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();

            MSWriteOptions msWriteOptions = new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };

            MSDataStore.Create(partition, documentId, document.ToString(), msWriteOptions, (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<Document<T>> PlatformReplace<T>(string partition, string documentId, T document)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();
            MSDataStore.Replace(partition, documentId, document.ToString(), (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<Document<T>> PlatformReplace<T>(string partition, string documentId, T document, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();

            MSWriteOptions msWriteOptions = new MSWriteOptions
            {
                DeviceTimeToLive = writeOptions.DeviceTimeToLive.Ticks
            };

            MSDataStore.Replace(partition, documentId, document.ToString(), msWriteOptions, (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    DocumentData = Document<T>.DeserializeString(resultDoc.DeserializedValue),
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }

        private static Task<Document<T>> PlatformDelete<T>(string partition, string documentId)
        {
            var taskCompletionSource = new TaskCompletionSource<Document<T>>();
            MSDataStore.Delete(partition, documentId, (resultDoc) =>
            {
                Document<T> doc = new Document<T>
                {
                    Partition = resultDoc.Partition,
                    Id = resultDoc.DocumentId,
                    ETag = resultDoc.ETag,
                    Timestamp = ((DateTime)resultDoc.LastUpdatedDate).Ticks,
                    IsFromCache = resultDoc.FromDeviceCache,
                    DocumentError = new Exception(resultDoc.Error.Error.LocalizedDescription)
                };

                taskCompletionSource.TrySetResult(doc);
            });
            return taskCompletionSource.Task;
        }
    }
}
