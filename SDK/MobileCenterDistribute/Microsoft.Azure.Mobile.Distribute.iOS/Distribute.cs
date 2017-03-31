using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Distribute
{
    using iOSDistribute = iOS.Bindings.MSDistribute;

    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(iOSDistribute);

        static bool PlatformEnabled
        {
            get
            {
                return iOSDistribute.IsEnabled();
            }

            set
            {
                iOSDistribute.SetEnabled(value);
            }
        }
        static void PlatformSetInstallUrl(string installUrl)
        {
            iOSDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            iOSDistribute.SetApiUrl(apiUrl);
        }

        /// <summary>
        /// Process URL request for the Distribute service.
        /// Place this method call into app delegate openUrl method.
        /// </summary>
        /// <param name="url">The url with parameters.</param>
        public static void OpenUrl(NSUrl url)
        {
            iOSDistribute.OpenUrl(url);
        }
    }
}
