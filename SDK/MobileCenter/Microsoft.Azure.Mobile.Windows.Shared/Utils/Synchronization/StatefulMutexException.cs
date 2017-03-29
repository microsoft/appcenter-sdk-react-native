using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
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
