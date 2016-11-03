using System;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile.iOS.Bindings
{
	[Native]
	public enum SNMLogLevel : ulong //was given as nuint, but i had to change to compile. not sure what is correct
	{
		Verbose = 2,
		Debug = 3,
		Info = 4,
		Warning = 5,
		Error = 6,
		Assert = 7,
		None = 99
	}
}
