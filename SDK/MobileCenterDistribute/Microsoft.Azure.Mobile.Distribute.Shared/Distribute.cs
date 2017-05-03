using System;

namespace Microsoft.Azure.Mobile.Distribute
{
    /// <summary>
    /// Distribute feature.
    /// </summary>
    public static partial class Distribute
    {
        /// <summary>
        /// Enable or disable Distribute module.
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
        /// Change the base URL opened in the browser to get update token from user login information.
        /// </summary>
        /// <param name="installUrl">Install base URL.</param>
        public static void SetInstallUrl(string installUrl)
        {
            PlatformSetInstallUrl(installUrl);
        }

        /// <summary>
        /// Change the base URL used to make API calls.
        /// </summary>
        /// <param name="apiUrl">API base URL.</param>
        public static void SetApiUrl(string apiUrl)
        {
            PlatformSetApiUrl(apiUrl);
        }

        /// <summary>
        /// Sets the release available callback.
        /// </summary>
        /// <value>The release available callback.</value>
        public static ReleaseAvailableCallback ReleaseAvailable
        {
            set
            {
                SetReleaseAvailableCallback(value);
            }
        }

        /// <summary>
        /// If update dialog is customized by returning <c>true</c> in <see cref="ReleaseAvailableCallback"/>,
        /// You need to tell the distribute SDK using this function what is the user action.
        /// </summary>
        /// <param name="updateAction">Update action. On mandatory update, you can only pass <see cref="UpdateAction.Update"/></param>
        public static void NotifyUpdateAction(UpdateAction updateAction)
        {
            HandleUpdateAction(updateAction);
        }
    }
}
