using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public interface IApplicationLifecycleHelper
    {
        bool Enabled { get; set; }
        event Action ApplicationSuspending;
        event Action ApplicationResuming;
    }
}
