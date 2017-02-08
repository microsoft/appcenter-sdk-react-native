using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannel
    {
        event EnqueuingLogEventHandler EnqueuingLog;
        event SendingLogEventHandler SendingLog;
        event SentLogEventHandler SentLog;
        event FailedToSendLogEventHandler FailedToSendLog;
    }
}
