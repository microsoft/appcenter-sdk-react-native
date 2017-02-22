using System;
using Microsoft.Rest;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public class IngestionException : MobileCenterException
    {
        private const string DefaultMessage = "The ingestion operation failed";

        public bool IsRecoverable
        {
            get
            {
                var httpException = InnerException as HttpOperationException;
                if (httpException == null)
                {
                    return false;
                }
                var statusCode = (int)httpException.Response.StatusCode;
                return statusCode >= 501 || statusCode == 408 || statusCode == 429 || statusCode == 401;
            }
        }

        public IngestionException(string message) : base(message) { }
        public IngestionException(string message, Exception innerException) : base(message, innerException) { }
        public IngestionException(Exception innerException) : base(DefaultMessage, innerException) { }
        public IngestionException() : base(DefaultMessage) { }
    }
}
