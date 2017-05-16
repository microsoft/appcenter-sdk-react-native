using Microsoft.Azure.Mobile.Push.Shared.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Utils.Synchronization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;

namespace Microsoft.Azure.Mobile.Push
{
    using WindowsPushNotificationReceivedEventArgs = Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs;

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
                            CustomData = customData ?? new Dictionary<string, string>() // TODO after backend updated to always use launch, filter push instead
                        });
                    }
                }
            }
        }

        private ApplicationLifecycleHelper _lifecycleHelper = new ApplicationLifecycleHelper();

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

        private void OnPushNotificationReceivedHandler(PushNotificationChannel sender, WindowsPushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == PushNotificationType.Toast)
            {
                var content = e.ToastNotification.Content;
                var notificationContent = content.GetXml();
                MobileCenterLog.Debug(LogTag, $"Received push notification payload: {notificationContent}");
                if (_lifecycleHelper.IsSuspended)
                {
                    MobileCenterLog.Debug(LogTag, "Application in background. Push callback will be called when user clicks the toast notification.");
                }
                else
                {
                    e.Cancel = true;
                    var notificationEventData = ExtractPushNotificationReceivedEventArgsFromXml(content);
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
            var pushNotification = new PushNotificationReceivedEventArgs();
            var messageNode = content.SelectSingleNode("/toast/visual/binding/text[@id='2']");
            if (messageNode != null)
            {
                pushNotification.Message = messageNode.InnerText;
                pushNotification.Title = content.SelectSingleNode("/toast/visual/binding/text[@id='1']")?.InnerText;
            }

            // TODO remove this when backend updated to use identifiers
            else
            {
                string firstText = null, secondText = null;
                foreach (var node in content.SelectNodes("/toast/visual/binding/text"))
                {
                    var text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (firstText == null)
                        {
                            firstText = text;
                        }
                        else
                        {
                            secondText = text;
                            break;
                        }
                    }
                }
                if (secondText == null)
                {
                    pushNotification.Message = firstText;
                }
                else
                {
                    pushNotification.Title = firstText;
                    pushNotification.Message = secondText;
                }
            }

            // TODO when backend modified to always send custom data, filter push with launch
            var launch = content.SelectSingleNode("/toast/@launch")?.NodeValue.ToString();
            pushNotification.CustomData = ParseLaunchString(launch) ?? new Dictionary<string, string>();
            return pushNotification;
        }

        private static Dictionary<string, string> ParseLaunchString(string launchString)
        {
            try
            {
                if (launchString != null)
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

        private static event EventHandler<PushNotificationReceivedEventArgs> PlatformPushNotificationReceived;
    }
}
