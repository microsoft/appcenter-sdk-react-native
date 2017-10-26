using System;
using ObjCRuntime;

namespace Microsoft.AppCenter.Distribute.iOS.Bindings
{
    [Native]
    public enum MSUpdateAction : ulong
    {
    	Update = 0,
    	Postpone = 1
    }
}
