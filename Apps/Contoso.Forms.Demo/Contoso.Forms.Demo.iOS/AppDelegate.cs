using Foundation;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;
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

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            Distribute.OpenUrl(url);
            return true;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Push.RegisteredForRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Push.FailedToRegisterForRemoteNotifications(error);
        }
    }
}
