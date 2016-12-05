using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Microsoft.Azure.Mobile;
using System.Runtime.InteropServices;
using System.IO;

namespace Contoso.Forms.Test.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
          global::Xamarin.Forms.Forms.Init();

            // Code for starting up the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            MobileCenter.Configure("f52dd054-e31f-4ff9-9f24-4e8d7942705b");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
