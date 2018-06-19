using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AppCenter.Push.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Synchronization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;

using WindowsPushNotificationReceivedEventArgs = Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs;

namespace Microsoft.AppCenter.Push
{
    public partial class Push
    {
        private PushNotificationChannel _channel;

        protected override int TriggerCount => 1;

        /// <summary>
        /// Call this method at the end of Application.OnLaunched with the same parameter as OnLaunched.
        /// This method call is needed to handle click on push to trigger the portable PushNotificationReceived event.
        /// </summary>
        /// <param name="e">OnLaunched method event args</param>
        public static void CheckLaunchedFromNotification(LaunchActivatedEventArgs e)
        {
            Instance.InstanceCheckLaunchedFromNotification(e?.Arguments);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InstanceCheckLaunchedFromNotification(string args)
        {
            IDictionary<string, string> customData = null;
            using (_mutex.GetLock())
            {
                if (!IsInactive)
                {
                    customData = ParseLaunchString(args);
                }
            }
            if (customData != null)
            {
                PushNotificationReceived?.Invoke(null, new PushNotificationReceivedEventArgs
                {
                    Title = null,
                    Message = null,
                    CustomData = customData
                });
            }
        }

        /// <summary>
        /// If enabled, register push channel and send URI to backend.
        /// Also start intercepting pushes.
        /// If disabled and previously enabled, stop listening for pushes (they will still be received though).
        /// </summary>
        private void ApplyEnabledState(bool enabled)
        {
            if (enabled)
            {
                // We expect caller of this method to lock on _mutex, we can't do it here as that lock is not recursive
                AppCenterLog.Debug(LogTag, "Getting push token...");
                var state = _mutex.State;
                CreatePushNotificationChannel(channel =>
                {
                    try
                    {
                        using (_mutex.GetLock(state))
                        {
                            var pushToken = channel?.Uri;
                            if (!string.IsNullOrEmpty(pushToken))
                            {
                                // Save channel member
                                _channel = channel;

                                // Subscribe to push
                                channel.PushNotificationReceived += OnPushNotificationReceivedHandler;

                                // Send channel URI to backend
                                AppCenterLog.Debug(LogTag, $"Push token '{pushToken}'");

                                var pushInstallationLog = new PushInstallationLog(null, null, pushToken, Guid.NewGuid());

                                // Do not await the call to EnqueueAsync or the UI thread can be blocked!
#pragma warning disable CS4014
                                Channel.EnqueueAsync(pushInstallationLog);
#pragma warning restore
                            }
                            else
                            {
                                AppCenterLog.Error(LogTag, "Push service registering with App Center backend has failed.");
                            }
                        }
                    }
                    catch (StatefulMutexException)
                    {
                        AppCenterLog.Warn(LogTag, "Push Enabled state changed after creating channel.");
                    }
                });
            }
            else if (_channel != null)
            {
                _channel.PushNotificationReceived -= OnPushNotificationReceivedHandler;
            }
        }

        private void OnPushNotificationReceivedHandler(PushNotificationChannel sender, WindowsPushNotificationReceivedEventArgs e)
        {
            XmlDocument content = null;
            if (e.NotificationType == PushNotificationType.Toast && (content = e.ToastNotification?.Content) != null)
            {
                AppCenterLog.Debug(LogTag, $"Received push notification payload: {content.GetXml()}");
                if (ApplicationLifecycleHelper.Instance.IsSuspended)
                {
                    AppCenterLog.Debug(LogTag, "Application in background. Push callback will be called when user clicks the toast notification.");
                }
                else
                {
                    var pushNotification = ParseAppCenterPush(content);
                    if (pushNotification != null)
                    {
                        e.Cancel = true;
                        PushNotificationReceived?.Invoke(sender, pushNotification);
                        AppCenterLog.Debug(LogTag, "Application in foreground. Intercept push notification and invoke push callback.");
                    }
                    else
                    {
                        AppCenterLog.Debug(LogTag, "Push ignored. It was not sent through App Center.");
                    }
                }
            }
            else
            {
                AppCenterLog.Debug(LogTag, $"Push ignored. We only handle Toast notifications but PushNotificationType is '{e.NotificationType}'.");
            }
        }

        private void CreatePushNotificationChannel(Action<PushNotificationChannel> created)
        {
            Task<PushNotificationChannel> CreatePushNotificationChannelForApplicationAsync()
            {
                try
                {
                    return new WindowsPushNotificationChannelManager()
                        .CreatePushNotificationChannelForApplicationAsync()
                        .AsTask();
                }
                catch (Exception e)
                {
                    return Task.FromException<PushNotificationChannel>(e);
                }
            }

            void OnNetworkStateChange(object sender, EventArgs e)
            {
                if (NetworkStateAdapter.IsConnected)
                {
                    NetworkStateAdapter.NetworkStatusChanged -= OnNetworkStateChange;
                    AppCenterLog.Debug(LogTag, "Second attempt to create notification channel...");

                    // Second attempt is the last one anyway.
                    CreatePushNotificationChannelForApplicationAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            AppCenterLog.Error(LogTag, "Unable to create notification channel.", task.Exception);
                            return;
                        }
                        created?.Invoke(task.Result);
                    });
                }
            }

            // If this isn't the first time after installation, the notification channel is created successfully even without network.
            CreatePushNotificationChannelForApplicationAsync().ContinueWith(task =>
            {
                if (task.IsFaulted && NetworkStateAdapter.IsConnected)
                {
                    AppCenterLog.Error(LogTag, "Unable to create notification channel.", task.Exception);
                    return;
                }
                if (!task.IsFaulted && task.Result != null)
                {
                    created?.Invoke(task.Result);
                    return;
                }
                AppCenterLog.Debug(LogTag, "The network isn't connected, another attempt to crate push notification channel " +
                                           "will be made after the network is available.");
                NetworkStateAdapter.NetworkStatusChanged += OnNetworkStateChange;
            });
        }

        internal static PushNotificationReceivedEventArgs ParseAppCenterPush(XmlDocument content)
        {
            // Check if app center push (it always has launch attribute with JSON object having mobile_center key)
            var launch = content.SelectSingleNode("/toast/@launch")?.NodeValue.ToString();
            var customData = ParseLaunchString(launch);
            if (customData == null)
            {
                return null;
            }

            // Parse title and message using identifiers
            return new PushNotificationReceivedEventArgs
            {
                Title = content.SelectSingleNode("/toast/visual/binding/text[@id='1']")?.InnerText,
                Message = content.SelectSingleNode("/toast/visual/binding/text[@id='2']")?.InnerText,
                CustomData = customData
            };
        }

        internal static Dictionary<string, string> ParseLaunchString(string launchString)
        {
            try
            {
                if (!string.IsNullOrEmpty(launchString))
                {
                    var launchJObject = JObject.Parse(launchString);
                    if (launchJObject?["appCenter"] is JObject appCenterData)
                    {
                        return ParseCustomData(appCenterData);
                    }
                    if (launchJObject?["mobile_center"] is JObject mobileCenterData)
                    {
                        return ParseCustomData(mobileCenterData);
                    }
                }
                return null;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        private static Dictionary<string, string> ParseCustomData(JObject appCenterData)
        {
            var customData = new Dictionary<string, string>();
            foreach (var pair in appCenterData)
            {
                customData.Add(pair.Key, pair.Value.ToString());
            }
            return customData;
        }
    }
}
