using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP
{
    public interface ILog { }
    public class Sender { }
}

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public delegate void SendingLogEventHandler(object sender, SendingLogEventArgs e);
    public delegate void SentLogEventHandler(object sender, SentLogEventArgs e);
    public delegate void FailedToSendLogEventHandler(object sender, FailedToSendLogEventArgs e);
}
