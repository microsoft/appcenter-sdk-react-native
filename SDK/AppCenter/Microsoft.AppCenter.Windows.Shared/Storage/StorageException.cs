using System;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageException : AppCenterException
    {
        private const string DefaultMessage = "The storage operation failed";
        public StorageException(string message) : base(message) { }
        public StorageException(string message, Exception innerException) : base(message, innerException) { }
        public StorageException(Exception innerException) : base(DefaultMessage, innerException) { }
        public StorageException() : base(DefaultMessage) { }
    }
}
