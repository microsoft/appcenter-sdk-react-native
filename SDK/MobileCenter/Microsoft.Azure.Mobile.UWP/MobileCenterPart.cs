using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile
{
    public partial class MobileCenter
    {
        private const string PlatformIdentifier = "uwp";

        /// <summary>
        /// Sets the device country to display in the portal
        /// </summary>
        /// <param name="country">The current country</param>
        public static void SetCountry(string country)
        {
            DeviceInformationHelper.SetCountry(country);
        }
    }
}
