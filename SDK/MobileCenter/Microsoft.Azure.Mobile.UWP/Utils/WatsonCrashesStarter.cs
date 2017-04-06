using WatsonRegistrationUtility;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// Used to register application with watson for Crashes.
    /// </summary>
    public class WatsonCrashesStarter
    {
        /// <exception cref="MobileCenterException"/>
        public static void RegisterWithWatson(string appSecret)
        {
            try
            {
                WatsonRegistrationManager.Start(appSecret);
            }
            catch
            {
                MobileCenterLog.Info(MobileCenterLog.LogTag, "The API to register this application with the Watson service is not supported on this device.");
            }
        }
    }
}
