using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Channel
{

    public class Channel : IChannel
    {
        public string Name { get; }

        public void Enqueue(ILog log)
        {
            // enqueue log
        }

        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        public void Clear()
        {
            //clear all logs
        }
    }
}
