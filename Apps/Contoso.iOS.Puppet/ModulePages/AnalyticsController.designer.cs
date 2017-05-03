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
	[Register ("AnalyticsController")]
	partial class AnalyticsController
	{
		[Outlet]
		UIKit.UISwitch AnalyticsEnabledSwitch { get; set; }

		[Outlet]
		UIKit.UITextField EventName { get; set; }

		[Outlet]
		UIKit.UILabel NumPropertiesLabel { get; set; }

		[Action ("AddProperty")]
		partial void AddProperty ();

		[Action ("TrackEvent")]
		partial void TrackEvent ();

		[Action ("UpdateEnabled")]
		partial void UpdateEnabled ();
		
		void ReleaseDesignerOutlets ()
		{
			if (AnalyticsEnabledSwitch != null) {
				AnalyticsEnabledSwitch.Dispose ();
				AnalyticsEnabledSwitch = null;
			}

			if (EventName != null) {
				EventName.Dispose ();
				EventName = null;
			}

			if (NumPropertiesLabel != null) {
				NumPropertiesLabel.Dispose ();
				NumPropertiesLabel = null;
			}
		}
	}
}
