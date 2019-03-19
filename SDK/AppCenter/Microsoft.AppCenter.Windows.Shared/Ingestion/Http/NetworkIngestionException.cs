// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;
    }
}
