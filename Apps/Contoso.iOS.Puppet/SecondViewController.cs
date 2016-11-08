using System;

using UIKit;

namespace Contoso.iOS.Puppet
{
	public partial class SecondViewController : UIViewController
	{
		protected SecondViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			//int er = 4 / int.Parse("0");

		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
