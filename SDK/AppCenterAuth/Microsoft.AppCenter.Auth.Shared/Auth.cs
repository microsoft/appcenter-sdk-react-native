// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.AppCenter.Auth
{
    /// <summary>
    /// Auth service.
    /// </summary>
    public partial class Auth
    {
        /// Sets the remote configuration base URL.
        /// </summary>
        /// <param name="configUrl">Remote configuration base URL.</param>
        public static void SetConfigUrl(string configUrl)
        {
            PlatformSetConfigUrl(configUrl);
        }

        /// <summary>
        /// Check whether the Auth service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        /// Enable or disable the Auth service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Sign in to get user information.
        /// </summary>
        /// <returns>User information.</returns>
        /// <exception cref="System.Exception">If sign-in failed.</exception>
        public static Task<UserInformation> SignInAsync()
        {
            return PlatformSignInAsync();
        }

        /// <summary>
        /// Sign out user and invalidate a user's token.
        /// </summary>
        public static void SignOut()
        {
            PlatformSignOut();
        }
    }
}
