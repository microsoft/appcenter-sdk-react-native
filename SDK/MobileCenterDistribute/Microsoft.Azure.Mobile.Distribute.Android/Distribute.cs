using System;
using Android.Runtime;
using Com.Microsoft.Azure.Mobile.Distribute;

namespace Microsoft.Azure.Mobile.Distribute
{
    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(AndroidDistribute);

        static bool PlatformEnabled
        {
            get
            {
                return AndroidDistribute.Enabled;
            }

            set
            {
                AndroidDistribute.Enabled = value;
            }
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
            AndroidDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            AndroidDistribute.SetApiUrl(apiUrl);
        }
    }
}
