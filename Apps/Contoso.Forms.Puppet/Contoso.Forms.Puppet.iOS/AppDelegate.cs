using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Puppet.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Sonoma must be initialized before the App is created
            MobileCenter.Initialize("44cd8722-bfe0-4748-ac14-7692e031a8a5");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);

        }
    }
}
