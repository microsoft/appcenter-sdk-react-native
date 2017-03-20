using System;

namespace Microsoft.Azure.Mobile.Storage
{
    internal class StorageException : MobileCenterException
    {
        private const string DefaultMessage = "The storage operation failed";
        public StorageException(string message) : base(message) { }
        public StorageException(string message, Exception innerException) : base(message, innerException) { }
        public StorageException(Exception innerException) : base(DefaultMessage, innerException) { }
        public StorageException() : base(DefaultMessage) { }
    }
}
