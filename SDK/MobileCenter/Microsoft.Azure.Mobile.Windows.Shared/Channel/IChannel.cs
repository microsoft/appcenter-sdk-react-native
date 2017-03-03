using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannel
    {
        event EnqueuingLogEventHandler EnqueuingLog;
        event SendingLogEventHandler SendingLog;
        event SentLogEventHandler SentLog;
        event FailedToSendLogEventHandler FailedToSendLog;
        void SetEnabled(bool enabled);
        void Shutdown();

        //TODO comment on how this is a fat interface
        void Enqueue(Log log);
    }
}
