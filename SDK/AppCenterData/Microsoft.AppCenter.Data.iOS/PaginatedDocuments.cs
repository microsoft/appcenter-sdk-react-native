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
        internal MSPaginatedDocuments internalDocuments { get; }

        public PaginatedDocuments(MSPaginatedDocuments iosDocuments)
        {
            internalDocuments = iosDocuments;
        }  

        bool HasNextPage
        {
            get { return internalDocuments.HasNextPage(); }
        }

        Page<T> CurrentPage
        {
            get
            {
                var source = internalDocuments.CurrentPage().Items;
                return GetPageFromInternalSource(source);
            }
        }

        Task<Page<T>> GetNextPage()
        {
            var taskCompletionSource = new TaskCompletionSource<Page<T>>();
            internalDocuments.NextPage((internalPage) =>
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
                page.Items.Add(Data.ConvertInternalDocToExternal(item));
            }
            return page;
        }
    }
}
