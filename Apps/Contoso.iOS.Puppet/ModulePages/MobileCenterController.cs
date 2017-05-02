using System;
using Microsoft.Azure.Mobile;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class MobileCenterController : UITableViewController
    {
        public MobileCenterController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            MobileCenterEnabledSwitch.On = MobileCenter.Enabled;
        }

        partial void UpdateEnabled()
        {
            MobileCenter.Enabled = MobileCenterEnabledSwitch.On;
            MobileCenterEnabledSwitch.On = MobileCenter.Enabled;
        }

        partial void WriteLog()
        {
        }
    }
}
