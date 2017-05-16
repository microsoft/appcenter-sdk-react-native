using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {

        private void InstanceRegister()
        {
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
