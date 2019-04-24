// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// A (paginated) list of documents from CosmosDB.
    /// </summary>
    public partial class PaginatedDocuments<T> : IEnumerable<DocumentWrapper<T>>
    {
        public IEnumerator<DocumentWrapper<T>> GetEnumerator()
        {
            while (true)
            {
                foreach (var item in CurrentPage?.Items ?? Enumerable.Empty<DocumentWrapper<T>>())
                {
                    yield return item;
                }
                if (!HasNextPage)
                {
                    yield break;
                }
                GetNextPageAsync().GetAwaiter().GetResult();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
