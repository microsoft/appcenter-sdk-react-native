using System;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class DistributeController : UITableViewController
    {
        public DistributeController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            DistributeEnabledSwitch.On = Distribute.IsEnabledAsync().Result;
            DistributeEnabledSwitch.Enabled = AppCenter.IsEnabledAsync().Result;
        }

        partial void UpdateEnabled()
        {
            Distribute.SetEnabledAsync(DistributeEnabledSwitch.On).Wait();
            DistributeEnabledSwitch.On = Distribute.IsEnabledAsync().Result;
        }
    }
}
