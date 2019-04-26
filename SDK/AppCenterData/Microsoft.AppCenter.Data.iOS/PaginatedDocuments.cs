// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AppCenter.Data.iOS.Bindings;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        private MSPaginatedDocuments PaginatedDocumentsInternal { get; }

        public PaginatedDocuments(MSPaginatedDocuments paginatedDocuments)
        {
            PaginatedDocumentsInternal = paginatedDocuments;
        }

        /// <summary>
        /// Boolean indicating if an extra page is available.
        /// </summary>
        /// <returns>True if there is another page of documents, false otherwise.</returns>
        public bool HasNextPage => PaginatedDocumentsInternal.HasNextPage(); 

        /// <summary>
        /// Return the current page.
        /// </summary>
        /// <returns>The current page of documents.</returns>
        public Page<T> CurrentPage => PaginatedDocumentsInternal.CurrentPage().ToPage<T>(); 

        /// <summary>
        /// Asynchronously fetch the next page.
        /// </summary>
        /// <returns>The next page of documents.</returns>
        public Task<Page<T>> GetNextPageAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Page<T>>();
            PaginatedDocumentsInternal.NextPage(internalPage =>
            {
                if (internalPage.Error == null)
                {
                    taskCompletionSource.SetResult(internalPage.ToPage<T>());
                }
                else
                {
                    taskCompletionSource.SetException(internalPage.Error.ToDataException());
                }
            });
            return taskCompletionSource.Task;
        }
    }
}
