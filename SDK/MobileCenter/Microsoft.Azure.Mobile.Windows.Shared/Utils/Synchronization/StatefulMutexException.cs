using System;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    /// <summary>
    /// Exception thrown when a StatefulMutex cannot acquire a lock
    /// </summary>
    /// <seealso cref="StatefulMutex"/> 
    public class StatefulMutexException : MobileCenterException
    {
        public StatefulMutexException(string message) : base(message)
        {
        }

        public StatefulMutexException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
