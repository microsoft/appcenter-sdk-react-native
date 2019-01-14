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

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserIdTextField { get; set; }


        [Action ("UpdateEnabled")]
        partial void UpdateEnabled ();


        [Action ("WriteLog")]
        partial void WriteLog ();

        [Action ("UpdateUserId:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UpdateUserId (UIKit.UITextField sender);

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

            if (LogWriteLevelLabel != null) {
                LogWriteLevelLabel.Dispose ();
                LogWriteLevelLabel = null;
            }

            if (LogWriteMessage != null) {
                LogWriteMessage.Dispose ();
                LogWriteMessage = null;
            }

            if (LogWriteTag != null) {
                LogWriteTag.Dispose ();
                LogWriteTag = null;
            }

            if (UserIdTextField != null) {
                UserIdTextField.Dispose ();
                UserIdTextField = null;
            }
        }
    }
}