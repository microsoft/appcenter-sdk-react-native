// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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
