// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter;
using Com.Microsoft.Appcenter.Identity;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth
    {
        [Preserve]
        public static Type BindingType => typeof(AndroidAuth);

        private static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidAuth.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidAuth.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        private static Task<UserInformation> PlatformSignInAsync()
        {
            var future = AndroidAuth.SignIn();
            return Task.Run(() =>
            {
                var signInResult = (SignInResult)future.Get();
                if (signInResult.Exception != null)
                {
                    // Keep the stacktrace clean.
                    ExceptionDispatchInfo.Capture(signInResult.Exception).Throw();
                }
                return new UserInformation
                {
                    AccountId = signInResult.UserInformation.AccountId
                };
            });
        }

        private static void PlatformSignOut()
        {
            AndroidAuth.SignOut();
        }

        private static void PlatformSetConfigUrl(string configUrl)
        {
            AndroidAuth.SetConfigUrl(configUrl);
        }
    }
}
