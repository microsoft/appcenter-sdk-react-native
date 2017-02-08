// Copyright (c) Microsoft Corporation.  All rights reserved.
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public class IngestionException : Exception { }

    public partial interface IIngestion
    {
        Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken));
        void Close();
        void SetServerUrl(string serverUrl); //TODO should this be a property?
    }
}

