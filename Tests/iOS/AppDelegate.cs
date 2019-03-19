// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics.iOS.Bindings;
using UIKit;

namespace Contoso.Forms.Test.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
          global::Xamarin.Forms.Forms.Init();

            /* Code for starting up the Xamarin Test Cloud Agent */
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif
            MSAnalytics.SetDelegate(new AnalyticsDelegate());
            AppCenter.Configure("f52dd054-e31f-4ff9-9f24-4e8d7942705b");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }

    public class AnalyticsDelegate : MSAnalyticsDelegate
    {
        public override void WillSendEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
            EventSharer.InvokeSendingEvent(LogToEventData(eventLog));
        }

        public override void DidSucceedSendingEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
            EventSharer.InvokeSentEvent(LogToEventData(eventLog));
        }

        public override void DidFailSendingEventLog(MSAnalytics analytics, MSEventLog eventLog, NSError error)
        {
            EventSharer.InvokeFailedToSendEvent(LogToEventData(eventLog));
        }

        EventData LogToEventData(MSEventLog eventLog)
        {
            var data = new EventData
            {
                Name = eventLog.Name
            };
            return data;
        }
    }
}
