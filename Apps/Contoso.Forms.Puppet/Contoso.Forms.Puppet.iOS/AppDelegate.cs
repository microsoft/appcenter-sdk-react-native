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

            Xamarin.Forms.Forms.ViewInitialized += (object sender, Xamarin.Forms.ViewInitializedEventArgs e) =>
            {
                if (e.View.StyleId != null)
                {
                    e.NativeView.AccessibilityIdentifier = e.View.StyleId;
                }
            };

#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            MobileCenter.Configure("44cd8722-bfe0-4748-ac14-7692e031a8a5");

            LoadApplication(new App());
            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
