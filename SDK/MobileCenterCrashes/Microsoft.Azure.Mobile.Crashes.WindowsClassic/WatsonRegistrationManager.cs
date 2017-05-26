using System.Runtime.InteropServices;

namespace WatsonRegistrationUtility
{
    internal class WatsonRegistrationManager
    {
        private const string WatsonAppSecretKey = "VSMCAppSecret";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int WerRegisterCustomMetadata([MarshalAs(UnmanagedType.LPWStr)]string key, [MarshalAs(UnmanagedType.LPWStr)]string value);

        public static void Start(string appSecret)
        {
            WerRegisterCustomMetadata(WatsonAppSecretKey, appSecret);
        }
    }
}
