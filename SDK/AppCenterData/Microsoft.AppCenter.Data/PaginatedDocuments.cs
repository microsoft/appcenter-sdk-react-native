// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        /// <summary>
        /// The current page.
        /// </summary>
        public Page<T> CurrentPage { get => null; }
        
        /// <summary>
        /// Boolean indicating if an extra page is available.
        /// </summary>
        public bool HasNextPage { get => false; }

        /// <summary>
        /// Asynchronously fetch the next page.
        /// </summary>
        public Task<Page<T>> GetNextPage()
        {
            return Task.FromResult<Page<T>>(null);
        }
    }
}