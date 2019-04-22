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
    }
}
