// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.Foundation;
using Windows.Networking.PushNotifications;

namespace Microsoft.AppCenter.Push
{
    interface IWindowsPushNotificationChannelManager
    {
        IAsyncOperation<PushNotificationChannel> CreatePushNotificationChannelForApplicationAsync();
    }

    class WindowsPushNotificationChannelManager
    {
        public IAsyncOperation<PushNotificationChannel> CreatePushNotificationChannelForApplicationAsync()
        {
            return PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
        }
    }
}
