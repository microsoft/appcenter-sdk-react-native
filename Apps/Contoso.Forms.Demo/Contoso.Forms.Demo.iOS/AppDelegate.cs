using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Demo.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            MobileCenter.Configure("2b3f11b1-6a90-4f28-9ae3-fc33b3c5d729");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);

        }
    }
}
