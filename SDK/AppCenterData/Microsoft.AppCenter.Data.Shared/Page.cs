// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.AppCenter.Data
{
    public class Page<T>
    {
        /// <summary>
        /// Documents in the page.
        /// </summary>
        public IList<DocumentWrapper<T>> Items { get; internal set; }

        internal Page()
        {
        }
    }
}
