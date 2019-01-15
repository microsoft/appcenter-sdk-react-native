using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter
{
    public partial class AppCenter
    {
        private const string PlatformIdentifier = "uwp";

        static void PlatformSetUserId(string userId)
        {
            AppCenterLog.Error(AppCenterLog.LogTag, "AppCenter.SetUserId is not supported on UWP.");
        }

        /// <summary>
        /// Sets the two-letter ISO country code to send to the backend.
        /// </summary>
        /// <param name="countryCode">The two-letter ISO country code. See <see href="https://www.iso.org/obp/ui/#search"/> for more information.</param>
        public static void SetCountryCode(string countryCode)
        {
            if (countryCode != null && countryCode.Length != 2)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "App Center accepts only the two-letter ISO country code.");
                return;
            }
            DeviceInformationHelper.SetCountryCode(countryCode);
        }
    }
}
