using Foundation;
using Microsoft.AppCenter.Distribute;
using UIKit;

namespace Contoso.Forms.Demo.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            Distribute.DontCheckForUpdatesInDebug();
            LoadApplication(new App());
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}
