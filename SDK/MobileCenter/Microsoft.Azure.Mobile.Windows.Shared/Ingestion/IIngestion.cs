// Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public interface IIngestion : IDisposable
    {
        IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs);
        Task ExecuteCallAsync(IServiceCall call);
        void Close();
        void SetLogUrl(string logUrl);
    }
}

