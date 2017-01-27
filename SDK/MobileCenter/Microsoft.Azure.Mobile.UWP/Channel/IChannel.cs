using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public interface IChannel
    {
        event SendingLogEventHandler SendingLog;
        event SentLogEventHandler SentLog;
        event FailedToSendLogEventHandler FailedToSendLog;
        void Clear(); // clear logs for the ichannel
    }
}
