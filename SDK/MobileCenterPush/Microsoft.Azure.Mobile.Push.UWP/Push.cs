using System;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        // Retrieve the push token from platform-specific Push Notification Service,
        // and later use the token to register with Mobile Center backend.
        private void InstanceRegister()
        {
            if (!Enabled)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Push service is not enabled.");
            }

            _stateKeeper.InvalidateState();
            var stateSnapshot = _stateKeeper.GetStateSnapshot();
            _mutex.Unlock();

            var pushNotificationChannel = Task.Run(() => CreatePushNotificationChannel()).Result;

            try
            {
                _mutex.Lock(stateSnapshot);
            }
            catch (StatefulMutexException e)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Push service registering with Mobile Center backend has failed", e);
                return;
            }
            finally
            {
                _mutex.Unlock();
            }

            var pushToken = pushNotificationChannel.Uri;

            if (!string.IsNullOrEmpty(pushToken))
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Push token '{pushToken}'");

                var pushInstallationLog = new PushInstallationLog(0, null, pushToken, Guid.NewGuid());

                Channel.Enqueue(pushInstallationLog);
            }
            else
            {
                MobileCenterLog.Error(LogTag, "Push service registering with Mobile Center backend has failed.");
            }

            _mutex.Unlock();
        }

        private async Task<PushNotificationChannel> CreatePushNotificationChannel()
        {
            PushNotificationChannel channel = await new WindowsPushNotificationChannelManager().CreatePushNotificationChannelForApplicationAsync();

            return channel;
        }
    }
}
