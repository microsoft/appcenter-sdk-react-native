using System;

using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class TabBarController : UITabBarController
    {
        public TabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Storyboard References are only available since iOS 9.0, so do it manually.
            ViewControllers = new UIViewController[] {
                UIStoryboard.FromName("MobileCenter", null).InstantiateViewController("MobileCenter"),
                UIStoryboard.FromName("Analytics", null).InstantiateViewController("Analytics"),
                UIStoryboard.FromName("Crashes", null).InstantiateViewController("Crashes"),
                UIStoryboard.FromName("Distribute", null).InstantiateViewController("Distribute")
            };
        }
    }
}

