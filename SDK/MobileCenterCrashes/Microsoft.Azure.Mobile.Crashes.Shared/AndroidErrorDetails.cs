using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class AndroidErrorDetails
    {
        public AndroidErrorDetails(object _Exception, string _ThreadName)
        {
            Exception = _Exception;
            ThreadName = _ThreadName;
        }

        public object Exception { get; }

        public string ThreadName { get; }
    }
}
