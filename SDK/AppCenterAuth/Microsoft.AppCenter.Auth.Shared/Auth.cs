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
        /// Sets the remote configuration base URL.
        /// </summary>
        /// <param name="configUrl">Remote configuration base URL.</param>
        public static void SetConfigUrl(string configUrl)
        {
            PlatformSetConfigUrl(configUrl);
        }
    }
}
