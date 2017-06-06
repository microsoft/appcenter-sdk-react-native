using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Push.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    using WindowsPushNotificationReceivedEventArgs = Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs;

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
            Instance.InstanceCheckLaunchedFromNotification(e);
        }

        private void InstanceCheckLaunchedFromNotification(LaunchActivatedEventArgs e)
        {
            IDictionary<string, string> customData = null;
            using (_mutex.GetLock())
            {
                if (!IsInactive)
                {
                    customData = ParseLaunchString(e?.Arguments);
                }
            }
            if (customData != null)
            {
                PushNotificationReceived?.Invoke(null, new PushNotificationReceivedEventArgs()
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
                var state = _mutex.State;
                Task.Run(async () =>
                {
                    var channel = await new WindowsPushNotificationChannelManager().CreatePushNotificationChannelForApplicationAsync()
                        .AsTask().ConfigureAwait(false);
                    try
                    {
                        using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                        {
                            var pushToken = channel.Uri;
                            if (!string.IsNullOrEmpty(pushToken))
                            {
                                // Save channel member
                                _channel = channel;

                                // Subscribe to push
                                channel.PushNotificationReceived += OnPushNotificationReceivedHandler;

                                // Send channel URI to backend
                                MobileCenterLog.Debug(LogTag, $"Push token '{pushToken}'");
                                var pushInstallationLog = new PushInstallationLog(0, null, pushToken, Guid.NewGuid());
                                await Channel.EnqueueAsync(pushInstallationLog).ConfigureAwait(false);
                            }
                            else
                            {
                                MobileCenterLog.Error(LogTag, "Push service registering with Mobile Center backend has failed.");
                            }
                        }
                    }
                    catch (StatefulMutexException)
                    {
                        MobileCenterLog.Warn(LogTag, "Push Enabled state changed after creating channel.");
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
            if (e.NotificationType == PushNotificationType.Toast)
            {
                var content = e.ToastNotification.Content;
                MobileCenterLog.Debug(LogTag, $"Received push notification payload: {content.GetXml()}");
                if (ApplicationLifecycleHelper.Instance.IsSuspended)
                {
                    MobileCenterLog.Debug(LogTag, "Application in background. Push callback will be called when user clicks the toast notification.");
                }
                else
                {
                    var pushNotification = ParseMobileCenterPush(content);
                    if (pushNotification != null)
                    {
                        e.Cancel = true;
                        PushNotificationReceived?.Invoke(sender, pushNotification);
                        MobileCenterLog.Debug(LogTag, "Application in foreground. Intercept push notification and invoke push callback.");
                    }
                    else
                    {
                        MobileCenterLog.Debug(LogTag, "Push ignored. It was not sent through Mobile Center.");
                    }
                }
            }
            else
            {
                MobileCenterLog.Debug(LogTag, $"Push ignored. We only handle Toast notifications but PushNotificationType is '{e.NotificationType}'.");
            }
        }

        internal static PushNotificationReceivedEventArgs ParseMobileCenterPush(XmlDocument content)
        {
            // Check if mobile center push (it always has launch attribute with JSON object having mobile_center key)
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
                    if (launchJObject?["mobile_center"] is JObject mobileCenterData)
                    {
                        var customData = new Dictionary<string, string>();
                        foreach (var pair in mobileCenterData)
                        {
                            customData.Add(pair.Key, pair.Value.ToString());
                        }
                        return customData;
                    }
                }
                return null;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }
    }
}
