using System;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public class IngestionException : MobileCenterException
    {
        private const string DefaultMessage = "The ingestion operation failed";

        public virtual bool IsRecoverable => false;

        public IngestionException(string message) : base(message) { }
        public IngestionException(Exception innerException) : base(DefaultMessage, innerException) { }
        public IngestionException() : base(DefaultMessage) { }
    }
}
