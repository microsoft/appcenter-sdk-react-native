using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Sonoma.Crashes.Shared;
using Microsoft.Sonoma.Crashes.iOS.Bindings;

namespace Microsoft.Sonoma.Crashes
{
	using iOSCrashes = iOS.Bindings.SNMCrashes;

	class PlatformCrashes : PlatformCrashesBase
	{
		public override Type BindingType => typeof(iOSCrashes);

		public override bool Enabled
        {
            get { return SNMCrashes.IsEnabled(); }
            set { SNMCrashes.SetEnabled(value); }
		}

		public override bool HasCrashedInLastSession => iOSCrashes.HasCrashedInLastSession;

		//public override void TrackException(Exception exception)
		//{
		//	throw new NotImplementedException();
		//}

        //TODO this just logs every exception possible, which is not the way crashes are handled in the native sdk
		static PlatformCrashes()
		{
            SNMCrashes.SetDelegate(new CrashDelegate());
            SNMCrashes.NotifyWithUserConfirmation(SNMUserConfirmation.Always);

		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}
	}

    class CrashDelegate : SNMCrashesDelegate
    {
        public override bool CrashesShouldProcessErrorReport(SNMCrashes crashes, SNMErrorReport errorReport)
        {
            return true;
        }
        public override SNMErrorAttachment AttachmentWithCrashes(SNMCrashes crashes, SNMErrorReport errorReport)
        {
            return null;
        }
    }
}