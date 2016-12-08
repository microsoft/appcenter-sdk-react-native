using Foundation;
using Microsoft.Azure.Mobile;
using UIKit;

namespace Contoso.Forms.Demo.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();

            MobileCenter.Configure("3415fe56-3655-426f-bad3-5b4cb9773efd");

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
