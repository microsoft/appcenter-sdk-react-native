using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Push
{
    public class PushNotificationReceivedEventArgs : EventArgs
    {
        public IDictionary<string, string> CustomData;
        public string Title;
        public string Message;
    }
}
