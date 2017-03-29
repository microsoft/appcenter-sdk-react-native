using System;

namespace Microsoft.Azure.Mobile.Distribute
{
    public static partial class Distribute
    {
        /// <summary>
        ///     Enable or disable Distribute module.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return PlatformEnabled;
            }

            set
            {
                PlatformEnabled = value;
            }
        }

        public static void SetInstallUrl(string installUrl)
        {
            PlatformSetInstallUrl(installUrl);
        }

        public static void SetApiUrl(string apiUrl)
        {
            PlatformSetApiUrl(apiUrl);
        }
    }
}
