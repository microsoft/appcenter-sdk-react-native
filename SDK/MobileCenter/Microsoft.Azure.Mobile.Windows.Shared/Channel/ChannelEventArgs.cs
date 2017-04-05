using System;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Channel
{
    public abstract class ChannelEventArgs : EventArgs
    {
        protected ChannelEventArgs(Log log)
        {
            Log = log;
        }
        public Log Log { get; protected set; }
    }
    public class EnqueuingLogEventArgs : ChannelEventArgs
    {
        public EnqueuingLogEventArgs(Log log) : base(log) { }
    }
    public class SendingLogEventArgs : ChannelEventArgs
    {
        public SendingLogEventArgs(Log log) : base(log) { }
    }
    public class SentLogEventArgs : ChannelEventArgs
    {
        public SentLogEventArgs(Log log) : base(log) { }
    }
    public class FailedToSendLogEventArgs : ChannelEventArgs
    {
        public FailedToSendLogEventArgs(Log log, Exception exception) : base(log)
        {
            Exception = exception;
        }

        public Exception Exception { get; protected set; }
    }
}
