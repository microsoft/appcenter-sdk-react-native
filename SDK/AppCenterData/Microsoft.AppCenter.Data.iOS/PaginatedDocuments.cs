// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Data.iOS.Bindings;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        internal MSPaginatedDocuments internalDocuments { get; }

        PaginatedDocuments(MSPaginatedDocuments iosDocuments)
        {
            internalDocuments = iosDocuments;
        }  

        bool PlatformHasNextPage()
        {
            return internalDocuments.HasNextPage();
        }

        Page PlatformCurrentPage()
        {
            return new Page(internalDocuments.CurrentPage());
        }

        Task<Page> PlatformNextPage()
        {
            var taskCompletionSource = new TaskCompletionSource<Page>();
            internalDocuments.NextPage((page) =>
            {
                taskCompletionSource.TrySetResult(new Page(page));
            });
            return taskCompletionSource.Task;
        }
    }
}
