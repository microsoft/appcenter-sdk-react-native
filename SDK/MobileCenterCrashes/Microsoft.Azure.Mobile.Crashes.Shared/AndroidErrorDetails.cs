using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class AndroidErrorDetails
    {
        public AndroidErrorDetails(object throwable, string threadName)
        {
            Exception = throwable;
            ThreadName = threadName;
        }

        public object Exception { get; }

        public string ThreadName { get; }
    }
}
