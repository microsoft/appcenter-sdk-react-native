// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Contoso.iOS.Puppet
{
    [Register ("DistributeController")]
    partial class DistributeController
    {
        [Outlet]
        UIKit.UISwitch DistributeEnabledSwitch { get; set; }


        [Action ("UpdateEnabled")]
        partial void UpdateEnabled ();

        void ReleaseDesignerOutlets ()
        {
            if (DistributeEnabledSwitch != null) {
                DistributeEnabledSwitch.Dispose ();
                DistributeEnabledSwitch = null;
            }
        }
    }
}