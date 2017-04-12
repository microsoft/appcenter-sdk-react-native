using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// Used to register application with watson for Crashes.
    /// </summary>
    public class WatsonCrashesStarter
    {
        private const string WatsonKey = "VSMCAppSecret";
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int WerRegisterCustomMetadata([MarshalAs(UnmanagedType.LPWStr)]string key, [MarshalAs(UnmanagedType.LPWStr)]string value);

        /// <exception cref="MobileCenterException"/>
        public static void RegisterWithWatson(string appSecret)
        {
            try
            {
                WerRegisterCustomMetadata(WatsonKey, appSecret);
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
}
