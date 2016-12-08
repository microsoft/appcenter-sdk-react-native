using Foundation;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics.iOS.Bindings;
using UIKit;

namespace Contoso.Forms.Puppet.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            MSAnalytics.SetDelegate(new AnalyticsDelegate());
            MobileCenter.SetServerUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Configure("b889c4f2-9ac2-4e2e-ae16-dae54f2c5899");

            LoadApplication(new App());
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }

    public class AnalyticsDelegate : MSAnalyticsDelegate
    {
        public override void WillSendEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
            System.Console.WriteLine("Will send event********");
        }

        public override void DidSucceedSendingEventLog(MSAnalytics analytics, MSEventLog eventLog)
        {
            System.Console.WriteLine("did send event********");
        }

        public override void DidFailSendingEventLog(MSAnalytics analytics, MSEventLog eventLog, NSError error)
        {
            System.Console.WriteLine("failed to send event********");
        }
    }
}
