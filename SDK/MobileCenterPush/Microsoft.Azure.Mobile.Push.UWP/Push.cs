using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.Azure.Mobile.Push.Windows.Shared;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        /// <summary>
        /// Event for push notification received
        /// </summary>
        public static event TypedEventHandler<object, PushNotificationEventArgs> PushNotificationReceived;

        public static void OnLaunched(IActivatedEventArgs e)
        {
            if (e.Kind == ActivationKind.ToastNotification)
            {
                ToastNotificationActivatedEventArgs args = e as ToastNotificationActivatedEventArgs;

                if (args.Argument != null)
                {
                    var customData = ParseLaunchString(args.Argument);
                    if (customData.Count > 0)
                    {
                        PushNotificationEventArgs notificationEventData = new PushNotificationEventArgs()
                        {
                            Title = null,
                            Message = null,
                            CustomData = customData
                        };

                        PushNotificationReceived?.Invoke(null, notificationEventData);
                    }
                }
            }
        }

        /// <summary>
        /// Application in background or not
        /// </summary>
        private bool applicationInBackground = false;

        // Retrieve the push token from platform-specific Push Notification Service,
        // and later use the token to register with Mobile Center backend.
        private void InstanceRegister()
        {
            if (!Enabled)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Push service is not enabled.");
            }

            SubscribeToApplicationInBackground();

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

            channel.PushNotificationReceived += OnPushNotification;

            return channel;
        }

        private void OnPushNotification(object sender, PushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == PushNotificationType.Toast)
            {
                XmlDocument content = e.ToastNotification.Content;
                string notificationContent = content.GetXml();

                MobileCenterLog.Debug(LogTag, $"Received push message: {notificationContent}");

                if (applicationInBackground)
                {
                    MobileCenterLog.Debug(LogTag, "Application in background. Push callback will be called when user clicks the toast notification.");
                }
                else
                { 
                    e.Cancel = true;
                    PushNotificationEventArgs notificationEventData = ParsePushNotificationEventArgs(content);
                    PushNotificationReceived?.Invoke(sender, notificationEventData);

                    MobileCenterLog.Debug(LogTag, "Application in foreground. Intercept push notification and invoke push callback.");
                }

            }
            else
            {
                MobileCenterLog.Debug(LogTag, $"We only handle Toast notifications but PushNotificationType is '{e.NotificationType}'.");
            }
        }

        private PushNotificationEventArgs ParsePushNotificationEventArgs(XmlDocument content)
        {
            string title = string.Empty;
            string message = string.Empty;

            if (content.DocumentElement.ChildNodes != null && content.DocumentElement.ChildNodes[0].NodeName == "visual")
            {
                var node = content.DocumentElement.ChildNodes[0];
                if (node.ChildNodes != null && node.ChildNodes[0].NodeName == "binding")
                {
                    var innerNode = node.ChildNodes[0];
                    if (innerNode.ChildNodes.Count == 2)
                    {
                        title = innerNode.ChildNodes[0].InnerText;
                        message = innerNode.ChildNodes[1].InnerText;
                    }
                }
            }

            Dictionary<string, string> customData = new Dictionary<string, string>();

            if (content.ChildNodes != null && content.ChildNodes.Count >= 2)
            {
                XmlNamedNodeMap attributes = content.ChildNodes[1].Attributes;
                if (attributes != null && attributes.Count >= 1)
                {
                    string launchString = attributes[0].InnerText;
                    customData = ParseLaunchString(launchString);
                }
            }

            return new PushNotificationEventArgs()
            {
                Title = title,
                Message = message,
                CustomData = customData
            };
        }

        private static Dictionary<string, string> ParseLaunchString(string launchString)
        {
            var customData = new Dictionary<string, string>();
            JObject launchJObject = JObject.Parse(launchString);
            if (launchJObject != null)
            {
                var mobileCenterData = (JObject)launchJObject["mobile_center"];
                if (mobileCenterData != null)
                {
                    foreach (var pair in mobileCenterData)
                    {
                        string key = pair.Key;
                        string value = (string)pair.Value;
                        customData.Add(key, value);
                    }
                }
            }
            return customData;
        }

        private void SubscribeToApplicationInBackground()
        {
            CoreApplication.LeavingBackground += (sender, e) =>
            {
                applicationInBackground = false;
            };
            CoreApplication.EnteredBackground += (sender, e) =>
            {
                applicationInBackground = true;
            };
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
