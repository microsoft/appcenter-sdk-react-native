using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class AndroidErrorDetails
    {
        public AndroidErrorDetails(object throwable, string threadName)
        {
            Throwable = throwable;
            ThreadName = threadName;
        }

        public object Throwable { get; }

        public string ThreadName { get; }
    }
}
