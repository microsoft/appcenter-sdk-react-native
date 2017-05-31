using Windows.ApplicationModel.Activation;
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
            if (countryCode != null && countryCode.Length != 2)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, $"MobileCenter accept only the two-letter ISO country code.");
                return;
            }
            DeviceInformationHelper.SetCountryCode(countryCode);
        }

        /// <summary>
        /// Certain scenarios require an additional setup step involving this method. To use this, call "MobileCenter.NotifyOnLaunched(e)" at the end of your OnLaunched method.
        /// </summary>
        /// <param name="e">Launch arguments.</param>
        public static void NotifyOnLaunched(LaunchActivatedEventArgs e)
        {
            Instance.InstanceNotifyOnLaunched(e);
        }

        public void InstanceNotifyOnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationLifecycleHelper.Instance.NotifyOnLaunched();

            foreach (var service in _services)
            {
                try
                {
                    service.NotifyOnLaunched(e);
                }
                catch (MobileCenterException ex)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag,
                        $"An error occurred when notifying service {service.ServiceName} of application launch.", ex);
                }
            }
        }
    }
}
