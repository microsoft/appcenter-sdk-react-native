using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class HttpServiceCall : ServiceCall
    {


        public HttpServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
            : base(ingestion, logs, appSecret, installId)
        {
        }
    }
}
