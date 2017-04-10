using System;
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
            catch (Exception e)
            {
#if DEBUG
                throw new MobileCenterException("Failed to register crashes with Watson", e);
#else
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on UWP.");
#endif
            }
        }
}
