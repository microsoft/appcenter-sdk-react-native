using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public abstract class ChannelEventArgs : EventArgs
    {
        public ChannelEventArgs(ILog log)
        {
            Log = log;
        }
        public ILog Log { get; protected set; }
    }
    public class EnqueuingLogEventArgs : ChannelEventArgs
    {
        public EnqueuingLogEventArgs(ILog log) : base(log) { }
    }
    public class SendingLogEventArgs : ChannelEventArgs
    {
        public SendingLogEventArgs(ILog log) : base(log) { }
    }
    public class SentLogEventArgs : ChannelEventArgs
    {
        public SentLogEventArgs(ILog log) : base(log) { }
    }
    public class FailedToSendLogEventArgs : ChannelEventArgs
    {
        public FailedToSendLogEventArgs(ILog log, Exception exception) : base(log)
        {
            Exception = exception;
        }

        public Exception Exception { get; protected set; }
    }
}
