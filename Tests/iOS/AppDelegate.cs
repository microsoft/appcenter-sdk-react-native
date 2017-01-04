using Foundation;
using UIKit;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics.iOS.Bindings;
using System.Collections.Generic;

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
            MobileCenter.Configure("f52dd054-e31f-4ff9-9f24-4e8d7942705b");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }

    public class AnalyticsDelegate : MSAnalyticsDelegate
    {
        public override void WillSendEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
            EventData.Name = eventLog.Name;
            EventData.Properties = new Dictionary<string, string>();

            if (eventLog.Properties != null)
            {
                foreach (NSString nsstringKey in eventLog.Properties.Keys)
                {
                    string strVal = eventLog.Properties.ValueForKey(nsstringKey).ToString();
                    string strKey = nsstringKey.ToString();

                    EventData.Properties.Add(strKey, strVal);
                }
            }
            EventData.Updated();
        }

        public override void DidSucceedSendingEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
        }

        public override void DidFailSendingEventLog(MSAnalytics analytics, MSEventLog eventLog, NSError error)
        {
        }
    }
}
