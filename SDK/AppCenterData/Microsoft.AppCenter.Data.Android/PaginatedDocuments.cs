// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Com.Microsoft.Appcenter.Data.Models;

namespace Microsoft.AppCenter.Data
{
    public partial class PaginatedDocuments<T>
    {
        private readonly AndroidPaginatedDocuments _paginatedDocuments;

        /// <summary>
        /// The current page.
        /// </summary>
        public Page<T> CurrentPage => _paginatedDocuments.CurrentPage.ToPage<T>(); 

        /// <summary>
        /// Boolean indicating if an extra page is available.
        /// </summary>
        public bool HasNextPage => _paginatedDocuments.HasNextPage; 

        /// <summary>
        /// Asynchronously fetch the next page.
        /// </summary>
        public Task<Page<T>> GetNextPageAsync()
        {
            var future = _paginatedDocuments.NextPage;
            return Task.Run(() =>
            {
                var page = (AndroidPage)future.Get();
                return page.ToPage<T>();
            });
        }

        internal PaginatedDocuments(AndroidPaginatedDocuments paginatedDocuments)
        {
            _paginatedDocuments = paginatedDocuments;
        }
    }
}
