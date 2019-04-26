// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AppCenter.Data.iOS.Bindings;
using ObjCRuntime;

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
        public static Type BindingType => typeof(MSData);

        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSData.IsEnabled());
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSData.SetEnabled(enabled);
            return Task.CompletedTask;
        }

        private static void PlatformSetTokenExchangeUrl(string tokenExchangeUrl)
        {
            MSData.SetTokenExchangeUrl(tokenExchangeUrl);
        }

        private static Task<DocumentWrapper<T>> PlatformReadAsync<T>(string documentId, string partition, ReadOptions readOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msReadOptions = readOptions.ToMSReadOptions();
            try
            {

                MSData.Read(documentId, new Class(typeof(T)), partition, msReadOptions, resultDoc =>
                {
                    ProcessResult(resultDoc, taskCompletionSource);
                });
            }
            catch (JsonException e)
            {
                taskCompletionSource.SetException(new DataException("Failed to read data object", e));
            }
            return taskCompletionSource.Task;
        }

        private static Task<PaginatedDocuments<T>> PlatformListAsync<T>(string partition)
        {
            var taskCompletionSource = new TaskCompletionSource<PaginatedDocuments<T>>();
            try
            {
                MSData.List(new Class(typeof(MSDictionaryDocument)), partition, resultPages =>
                {
                    if (resultPages.CurrentPage().Error == null)
                    {
                        //TODO need to change each page document from dictionary document to T
                        taskCompletionSource.SetResult(resultPages.ToPaginatedDocuments<T>());
                    }
                    else
                    {
                        taskCompletionSource.SetException(resultPages.CurrentPage().Error.ToDataException());
                    }
                });
            }
            catch (JsonException e)
            {
                taskCompletionSource.SetException(new DataException("Failed to list data object(s)", e));
            }
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformCreateAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            try
            {
                MSData.Create(documentId, document.ToMSDocument(), partition, msWriteOptions, resultDoc =>
                {
                    ProcessResult(resultDoc, taskCompletionSource);
                });
            }
            catch (JsonException e)
            {
                taskCompletionSource.SetException(new DataException("Failed to create data object", e));
            }
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            try 
            {
                MSData.Replace(documentId, document.ToMSDocument(), partition, msWriteOptions, resultDoc =>
                {
                    ProcessResult(resultDoc, taskCompletionSource);
                });
            }
            catch (JsonException e)
            {
                taskCompletionSource.SetException(new DataException("Failed to replace data object", e));
            }
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            try
            {
                MSData.Delete(documentId, partition, msWriteOptions, resultDoc =>
                {
                    ProcessResult(resultDoc, taskCompletionSource);
                });
            }
            catch (JsonException e)
            {
                taskCompletionSource.SetException(new DataException("Failed to delete data object", e));
            }
            return taskCompletionSource.Task;
        }

        private static void ProcessResult<T>(MSDocumentWrapper resultDoc, TaskCompletionSource<DocumentWrapper<T>> taskCompletionSource)
        {
            if (resultDoc.Error == null)
            {
                taskCompletionSource.SetResult(resultDoc.ToDocumentWrapper<T>());
            }
            else
            {
                taskCompletionSource.SetException(resultDoc.Error.ToDataException(resultDoc));
            }
        }
    }
}
