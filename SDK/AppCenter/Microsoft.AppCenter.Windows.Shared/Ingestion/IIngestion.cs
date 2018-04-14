// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion
{
    /// <summary>
    /// Interface to send logs to the Ingestion service.
    /// </summary>
    public interface IIngestion : IDisposable
    {
        /// <summary>
        /// Update log URL.
        /// </summary>
        /// <param name="logUrl"></param>
        void SetLogUrl(string logUrl);

        /// <summary>
        /// Send logs to the Ingestion service.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application</param>
        /// <param name="installId">Install identifier</param>
        /// <param name="logs">Payload</param>
        IServiceCall Call(string appSecret, Guid installId, IList<Log> logs);

        /// <summary>
        /// Close all current calls.
        /// </summary>
        void Close();
    }
}
