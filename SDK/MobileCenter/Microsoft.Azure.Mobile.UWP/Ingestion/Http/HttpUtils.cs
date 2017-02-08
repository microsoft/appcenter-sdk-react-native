using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public static class HttpUtils
    {
        public static bool IsRecoverableError(Exception exception)
        {
            var httpException = exception as HttpOperationException;
            if (httpException == null)
            {
                return false;//TODO what about other recoverable exceptiont that aren't of this type?
            }
            int statusCode = (int)httpException.Response.StatusCode;
            return statusCode >= 501 || statusCode == 408 || statusCode == 429 || statusCode == 401;
        }
    }
}
