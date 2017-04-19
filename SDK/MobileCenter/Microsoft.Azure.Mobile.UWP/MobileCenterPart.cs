using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile
{
    public partial class MobileCenter
    {
        private const string PlatformIdentifier = "uwp";

        /// <summary>
        /// Sets the two-letter ISO country code to send to the backend.
        /// </summary>
        /// <param name="countryCode">The two-letter ISO country code. See <see href="https://www.iso.org/obp/ui/#search"/> for more information.</param>
        public static void SetCountryCode(string countryCode)
        {
            DeviceInformationHelper.SetCountryCode(countryCode);
        }
    }
}
