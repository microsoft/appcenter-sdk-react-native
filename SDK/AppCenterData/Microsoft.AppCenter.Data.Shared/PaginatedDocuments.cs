// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Data
{
    public class PaginatedDocuments<T> : IEnumerable<Document<T>>
    {
        private List<Document<T>> documents;

        public PaginatedDocuments()
        {
            this.documents = new List<Document<T>>();
        }

        public void Add(Document<T> document)
        {
            this.documents.Add(document);
        }

        public IEnumerator<Document<T>> GetEnumerator()
        {
            return this.documents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.documents.GetEnumerator();
        }
    }
}
