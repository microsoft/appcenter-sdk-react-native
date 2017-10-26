using System;
using ObjCRuntime;

namespace Microsoft.AppCenter.Crashes.iOS.Bindings {
	[Native]
	public enum MSErrorLogSetting : ulong
	{
		Disabled = 0,
		AlwaysAsk = 1,
		AutoSend = 2
	}

	[Native]
	public enum MSUserConfirmation : ulong
	{
		DontSend = 0,
		Send = 1,
		Always = 2
	}
}
