// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                foreach (var i in CurrentPage?.Items ?? Enumerable.Empty<DocumentWrapper<T>>())
                {
                    yield return i;
                }
                if (!HasNextPage)
                {
                    yield break;
                }
                GetNextPage().Wait();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
