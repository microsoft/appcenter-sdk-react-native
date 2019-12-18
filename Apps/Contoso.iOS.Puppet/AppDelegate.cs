// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version

using System;
using System.Linq;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using UIKit;
using UserNotifications;

namespace Contoso.iOS.Puppet
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        static bool _didTapNotification;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.Delegate = new PushDelegate();
            }
            Push.PushNotificationReceived += (sender, e) =>
            {
                string message = "";
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    message += _didTapNotification ? "Tapped notification\n" : "Received in foreground\n";
                }
                message += e.Message ?? "";
                if (e.CustomData?.Count > 0)
                {
                    message += (e.Message?.Length > 0 ? "\n" : "") + "Custom data = {" + string.Join(",", e.CustomData.Select(kv => kv.Key + "=" + kv.Value)) + "}";
                }
                var alertController = UIAlertController.Create(e.Title, message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                Window.RootViewController.PresentViewController(alertController, true, null);
                _didTapNotification = false;
            };
            Distribute.SetInstallUrl("https://install.portal-server-core-integration.dev.avalanch.es");
            Distribute.SetApiUrl("https://api-gateway-core-integration.dev.avalanch.es/v0.1");
            Distribute.DontCheckForUpdatesInDebug();
            AppCenter.Start("e94aaff4-e80d-4fee-9a5f-a84eb6e688fc", typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
            return true;
        }

        class PushDelegate : UNUserNotificationCenterDelegate
        {
            public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
            {
                var push = notification.Request.Content.UserInfo;
                var appCenter = push["mobile_center"] as NSDictionary;
                var presentation = appCenter?["presentation"] as NSString;
                if (presentation != null && presentation.Equals((NSString)"alert"))
                {
                    completionHandler(UNNotificationPresentationOptions.Alert);
                }
                else
                {
                    Push.DidReceiveRemoteNotification(push);
                    completionHandler(UNNotificationPresentationOptions.None);
                }
            }

            public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
            {
                _didTapNotification = response.IsDefaultAction;
                Push.DidReceiveRemoteNotification(response.Notification.Request.Content.UserInfo);
                completionHandler();
            }
        }
    }
}

