using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    internal static class HttpUtils
    {
        internal static bool IsRecoverableError(IngestionException exception)
        {
            var httpException = exception.InnerException as HttpOperationException;
            if (httpException == null)
            {
                return false;//TODO what about other recoverable exceptions that aren't of this type?
            }
            var statusCode = (int)httpException.Response.StatusCode;
            return statusCode >= 501 || statusCode == 408 || statusCode == 429 || statusCode == 401;
        }
    }
}
