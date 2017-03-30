namespace Microsoft.Azure.Mobile.Distribute
{
    /// <summary>
    ///     Distribute feature.
    /// </summary>
    public static partial class Distribute
    {
        /// <summary>
        ///     Enable or disable Distribute module.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return PlatformEnabled;
            }

            set
            {
                PlatformEnabled = value;
            }
        }

        /// <summary>
        ///     Change the base URL opened in the browser to get update token from user login information.
        /// </summary>
        /// <param name="installUrl">Install base URL.</param>
        public static void SetInstallUrl(string installUrl)
        {
            PlatformSetInstallUrl(installUrl);
        }

        /// <summary>
        ///     Change the base URL used to make API calls.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        public static void SetApiUrl(string apiUrl)
        {
            PlatformSetApiUrl(apiUrl);
        }
    }
}
