using System;
#if REFERENCE
#else
using WatsonRegistrationUtility;
#endif

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
            MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on UWP.");
            try
            {
#if REFERENCE
#else
                WatsonRegistrationManager.Start(appSecret);
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                throw new MobileCenterException("Failed to register crashes with Watson", e);
#endif
            }
        }
    }
}
