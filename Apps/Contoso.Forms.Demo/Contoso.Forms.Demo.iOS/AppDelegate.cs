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

            MobileCenter.Configure("fe2bf05d-f4f9-48a6-83d9-ea8033fbb644");

            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
