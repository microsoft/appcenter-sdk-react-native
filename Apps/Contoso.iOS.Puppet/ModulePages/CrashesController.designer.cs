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
	[Register ("CrashesController")]
	partial class CrashesController
	{
		[Outlet]
		UIKit.UISwitch CrashesEnabledSwitch { get; set; }

		[Action ("CatchNullReferenceException")]
		partial void CatchNullReferenceException ();

		[Action ("CrashAsync")]
		partial void CrashAsync ();

		[Action ("CrashWithAggregateException")]
		partial void CrashWithAggregateException ();

		[Action ("CrashWithNullReferenceException")]
		partial void CrashWithNullReferenceException ();

		[Action ("DivideByZero")]
		partial void DivideByZero ();

		[Action ("TestCrash")]
		partial void TestCrash ();

		[Action ("UpdateEnabled")]
		partial void UpdateEnabled ();
		
		void ReleaseDesignerOutlets ()
		{
			if (CrashesEnabledSwitch != null) {
				CrashesEnabledSwitch.Dispose ();
				CrashesEnabledSwitch = null;
			}
		}
	}
}
