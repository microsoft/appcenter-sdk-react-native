using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.Mobile.Crashes.Shared;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOSCrashes = iOS.Bindings.MSCrashes;

    class PlatformCrashes : PlatformCrashesBase
    {
        public override Type BindingType => typeof(iOSCrashes);

        public override bool Enabled
        {
            get { return MSCrashes.IsEnabled(); }
            set { MSCrashes.SetEnabled(value); }
        }

        public override bool HasCrashedInLastSession => iOSCrashes.HasCrashedInLastSession;

        //public override void TrackException(Exception exception)
        //{
        //	throw new NotImplementedException();
        //}

        //TODO this just logs every exception possible, which is not the way crashes are handled in the native sdk
        static PlatformCrashes()
        {
            MSCrashes.SetDelegate(new CrashDelegate());
            MSCrashes.NotifyWithUserConfirmation(MSUserConfirmation.Always);

        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    class CrashDelegate : MSCrashesDelegate
    {
        public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport errorReport)
        {
            return true;
        }
        public override MSErrorAttachment AttachmentWithCrashes(MSCrashes crashes, MSErrorReport errorReport)
        {
            return null;
        }
    }
}