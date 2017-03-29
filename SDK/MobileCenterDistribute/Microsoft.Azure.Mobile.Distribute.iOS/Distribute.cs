using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Distribute
{
    using iOSDistribute = iOS.Bindings.MSDistribute;

    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(iOSDistribute);

        static bool PlatformEnabled { get; set; }

        static void PlatformSetInstallUrl(string installUrl)
        {
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
        }
    }
}
