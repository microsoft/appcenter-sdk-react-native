// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AppCenter.Data.iOS.Bindings;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        private MSPaginatedDocuments InternalDocuments { get; }

        public PaginatedDocuments(MSPaginatedDocuments iosDocuments)
        {
            InternalDocuments = iosDocuments;
        }

        /// <summary>
        /// Boolean indicating if an extra page is available.
        /// </summary>
        /// <returns>True if there is another page of documents, false otherwise.</returns>
        public bool HasNextPage
        {
            get => InternalDocuments.HasNextPage();
        }

        /// <summary>
        /// Return the current page.
        /// </summary>
        /// <returns>The current page of documents.</returns>
        public Page<T> CurrentPage
        {
            get => InternalDocuments.CurrentPage().ToPage<T>();
        }

        /// <summary>
        /// Asynchronously fetch the next page.
        /// </summary>
        /// <returns>The next page of documents.</returns>
        public Task<Page<T>> GetNextPageAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Page<T>>();
            InternalDocuments.NextPage(internalPage =>
            {
                if (internalPage.Error != null)
                {
                    taskCompletionSource.TrySetException(internalPage.Error.ToDataException());
                }
                else
                {
                    taskCompletionSource.TrySetResult(internalPage.ToPage<T>());
                }
            });
            return taskCompletionSource.Task;
        }
    }
}
