using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public abstract class ChannelEventArgs : EventArgs
    {
        public ILog Log { get; protected set; }
    }
    public class SendingLogEventArgs : ChannelEventArgs { }
    public class SentLogEventArgs : ChannelEventArgs { }
    public class FailedToSendLogEventArgs : ChannelEventArgs
    {
        public Exception Exception { get; protected set; }
    }
}
