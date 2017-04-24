using System;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        private async void InstanceRegister()
        {
            if (Enabled)
            {
                string pushToken = (await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync()).Uri;

                if (!string.IsNullOrEmpty(pushToken))
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Push token '{pushToken}'");

                    PushInstallationLog pushInstallationLog = new PushInstallationLog(0, null, pushToken, Guid.NewGuid());

                    Channel.Enqueue(pushInstallationLog);
                }
                else
                {
                    MobileCenterLog.Error(LogTag, "PushToken cannot be null or empty.");
                }
            }
            else
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Push service is not enable.");
            }
        }
    }
}
