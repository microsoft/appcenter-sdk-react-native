using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Push
{
    public partial class Push : MobileCenterService
    {
        public static void CheckPushActivation(IActivatedEventArgs e)
        {
            if (PlatformPushNotificationReceived != null)
            {
                // Depending on template, the event type and application override method differ.
                var argument = (e as ToastNotificationActivatedEventArgs)?.Argument ?? (e as LaunchActivatedEventArgs)?.Arguments;
                if (!string.IsNullOrEmpty(argument) || e.Kind == ActivationKind.ToastNotification)
                {
                    var customData = ParseLaunchString(argument);
                    if (customData != null || e.Kind == ActivationKind.ToastNotification)
                    {
                        PlatformPushNotificationReceived.Invoke(null, new PushNotificationReceivedEventArgs()
                        {
                            Title = null,
                            Message = null,
                            CustomData = customData ?? new Dictionary<string, string>()
                        });
                    }
                }
            }
        }

        private bool ApplicationInBackground = false;

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

            var pushNotificationChannel = Task.Run(async () =>
            {
                var channel = await new WindowsPushNotificationChannelManager().CreatePushNotificationChannelForApplicationAsync();

                channel.PushNotificationReceived += OnPushNotificationReceivedHandler;

                return channel;
            }).Result;

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

        private void OnPushNotificationReceivedHandler(Windows.Networking.PushNotifications.PushNotificationChannel sender,
            Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == Windows.Networking.PushNotifications.PushNotificationType.Toast)
            {
                XmlDocument content = e.ToastNotification.Content;
                string notificationContent = content.GetXml();

                MobileCenterLog.Debug(LogTag, $"Received push notification payload: {notificationContent}");

                if (ApplicationInBackground)
                {
                    MobileCenterLog.Debug(LogTag, "Application in background. Push callback will be called when user clicks the toast notification.");
                }
                else
                {
                    e.Cancel = true;
                    PushNotificationReceivedEventArgs notificationEventData = ExtractPushNotificationReceivedEventArgsFromXml(content);
                    PlatformPushNotificationReceived?.Invoke(sender, notificationEventData);

                    MobileCenterLog.Debug(LogTag, "Application in foreground. Intercept push notification and invoke push callback.");
                }
            }
            else
            {
                MobileCenterLog.Debug(LogTag, $"Ignored. We only handle Toast notifications but PushNotificationType is '{e.NotificationType}'.");
            }
        }

        private PushNotificationReceivedEventArgs ExtractPushNotificationReceivedEventArgsFromXml(XmlDocument content)
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

            return new PushNotificationReceivedEventArgs()
            {
                Title = title,
                Message = message,
                CustomData = customData
            };
        }

        private static Dictionary<string, string> ParseLaunchString(string launchString)
        {
            try
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
                return null;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        private void SubscribeToApplicationInBackground()
        {
            CoreApplication.LeavingBackground += (sender, e) =>
            {
                ApplicationInBackground = false;
            };
            CoreApplication.EnteredBackground += (sender, e) =>
            {
                ApplicationInBackground = true;
            };
        }

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
