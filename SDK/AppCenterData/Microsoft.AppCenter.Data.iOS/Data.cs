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
            MSData.Read(documentId, new Class(typeof(MSDictionaryDocument)), partition, msReadOptions, resultDoc =>
            {
                ProcessResult(resultDoc, taskCompletionSource);
            });
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
            MSData.Create(documentId, document.ToMSDocument(), partition, msWriteOptions, resultDoc =>
            {
                ProcessResult(resultDoc, taskCompletionSource);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformReplaceAsync<T>(string documentId, T document, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            MSData.Replace(documentId, document.ToMSDocument(), partition, msWriteOptions, resultDoc =>
            {
                ProcessResult(resultDoc, taskCompletionSource);
            });
            return taskCompletionSource.Task;
        }

        private static Task<DocumentWrapper<T>> PlatformDeleteAsync<T>(string documentId, string partition, WriteOptions writeOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<DocumentWrapper<T>>();
            var msWriteOptions = writeOptions.ToMSWriteOptions();
            MSData.Delete(documentId, partition, msWriteOptions, resultDoc =>
            {
                ProcessResult(resultDoc, taskCompletionSource);
            });
            return taskCompletionSource.Task;
        }

        private static void ProcessResult<T>(MSDocumentWrapper resultDoc, TaskCompletionSource<DocumentWrapper<T>> taskCompletionSource)
        {
            if (resultDoc.Error == null)
            {
                try
                {
                    taskCompletionSource.SetResult(resultDoc.ToDocumentWrapper<T>());
                }
                catch (JsonException e)
                {
                    taskCompletionSource.SetException(new DataException("Failed to process doc: serialization failed", e));
                }
            }
            else
            {
                taskCompletionSource.SetException(resultDoc.Error.ToDataException(resultDoc));
            }
        }
    }
}
