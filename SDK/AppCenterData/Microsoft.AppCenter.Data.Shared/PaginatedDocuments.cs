// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Data
{
    public class PaginatedDocuments<T> : IEnumerable<DocumentWrapper<T>>
    {
        private List<DocumentWrapper<T>> documents;

        public PaginatedDocuments()
        {
            this.documents = new List<DocumentWrapper<T>>();
        }

        public void Add(DocumentWrapper<T> document)
        {
            this.documents.Add(document);
        }

        public IEnumerator<DocumentWrapper<T>> GetEnumerator()
        {
            return this.documents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.documents.GetEnumerator();
        }
    }
}
