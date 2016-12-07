using Foundation;
using Microsoft.Azure.Mobile;
using UIKit;

namespace Contoso.Forms.Puppet.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();

            MobileCenter.SetServerUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Configure("b889c4f2-9ac2-4e2e-ae16-dae54f2c5899");

            LoadApplication(new App());
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
