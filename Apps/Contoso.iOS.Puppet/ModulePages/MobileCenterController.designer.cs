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
	[Register ("AppCenterController")]
	partial class AppCenterController
	{
		[Outlet]
		UIKit.UILabel LogLevelLabel { get; set; }

		[Outlet]
		UIKit.UILabel LogWriteLevelLabel { get; set; }

		[Outlet]
		UIKit.UITextField LogWriteMessage { get; set; }

		[Outlet]
		UIKit.UITextField LogWriteTag { get; set; }

		[Outlet]
		UIKit.UISwitch AppCenterEnabledSwitch { get; set; }

		[Action ("UpdateEnabled")]
		partial void UpdateEnabled ();

		[Action ("WriteLog")]
		partial void WriteLog ();
		
		void ReleaseDesignerOutlets ()
		{
			if (AppCenterEnabledSwitch != null) {
				AppCenterEnabledSwitch.Dispose ();
				AppCenterEnabledSwitch = null;
			}

			if (LogLevelLabel != null) {
				LogLevelLabel.Dispose ();
				LogLevelLabel = null;
			}

			if (LogWriteMessage != null) {
				LogWriteMessage.Dispose ();
				LogWriteMessage = null;
			}

			if (LogWriteTag != null) {
				LogWriteTag.Dispose ();
				LogWriteTag = null;
			}

			if (LogWriteLevelLabel != null) {
				LogWriteLevelLabel.Dispose ();
				LogWriteLevelLabel = null;
			}
		}
	}
}
