using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class HttpServiceCall : ServiceCall
    {
        public HttpServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
            : base(ingestion, logs, appSecret, installId)
        {
        }
    }
}
