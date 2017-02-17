using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Ingestion
{
    internal class IngestionException : MobileCenterException
    {
        private const string DefaultMessage = "The ingestion operation failed";

        public IngestionException(string message) : base(message) { }
        public IngestionException(string message, Exception innerException) : base(message, innerException) { }
        public IngestionException(Exception innerException) : base(DefaultMessage, innerException) { }
        public IngestionException() : base(DefaultMessage) { }
    }
}
