using System;
using ObjCRuntime;

namespace Microsoft.Sonoma.Crashes.iOS.Bindings {
	[Native]
	public enum SNMErrorLogSetting : ulong
	{
		Disabled = 0,
		AlwaysAsk = 1,
		AutoSend = 2
	}

	[Native]
	public enum SNMUserConfirmation : ulong
	{
		DontSend = 0,
		Send = 1,
		Always = 2
	}
}
