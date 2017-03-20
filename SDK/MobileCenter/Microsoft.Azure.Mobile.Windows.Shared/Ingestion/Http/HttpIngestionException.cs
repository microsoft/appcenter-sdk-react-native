using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class HttpIngestionException : IngestionException
    {
        public HttpMethod Method { get; set; }
        public Uri RequestUri { get; set; }
        public string RequestContent { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public override bool IsRecoverable
        {
            get
            {
                var statusCode = (int)StatusCode;
                return statusCode >= 501 || statusCode == 408 || statusCode == 429;
            }
        }

        public HttpIngestionException(string message) : base(message)
        {
        }
    }
}
