// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AppCenter.Data.iOS.Bindings;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        internal MSPaginatedDocuments InternalDocuments { get; }

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
            get { return InternalDocuments.HasNextPage(); }
        }

        /// <summary>
        /// Return the current page.
        /// </summary>
        /// <returns>The current page of documents.</returns>
        public Page<T> CurrentPage
        {
            get
            {
                var source = InternalDocuments.CurrentPage().Items;
                return GetPageFromInternalSource(source);
            }
        }

        /// <summary>
        /// Asynchronously fetch the next page.
        /// </summary>
        /// <returns>The next page of documents.</returns>
        public Task<Page<T>> GetNextPage()
        {
            var taskCompletionSource = new TaskCompletionSource<Page<T>>();
            InternalDocuments.NextPage((internalPage) =>
            {
                var source = internalPage.Items;
                taskCompletionSource.TrySetResult(GetPageFromInternalSource(source));
            });
            return taskCompletionSource.Task;
        }

        private Page<T> GetPageFromInternalSource(MSDocumentWrapper[] source) 
        {
            var page = new Page<T>();
            foreach (var item in source)
            {
                page.Items.Add(Data.ConvertInternalDocToExternal<T>(item));
            }
            return page;
        }
    }
}
