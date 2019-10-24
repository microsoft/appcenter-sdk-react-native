// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;

namespace Microsoft.AppCenter.Crashes.Windows.Utils
{
    /// <summary>
    /// This class is a data-only holder for the model exception and binary sent with error logs. A simple pair cannot be used because value tuples require an additional dependency in UWP.
    /// </summary>
    internal class ErrorExceptionAndBinaries
    {
        public ModelException Exception;
        public IList<Binary> Binaries;
    }
}
