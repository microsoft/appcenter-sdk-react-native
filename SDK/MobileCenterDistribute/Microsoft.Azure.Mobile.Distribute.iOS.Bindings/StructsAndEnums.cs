using System;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile.Distribute.iOS.Bindings
{
    [Native]
    public enum MSUpdateAction : ulong
    {
    	Update = 0,
    	Postpone = 1
    }
}
