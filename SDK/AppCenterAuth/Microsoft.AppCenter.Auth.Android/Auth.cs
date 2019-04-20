// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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

        private static Task<UserInformation> PlatformSignInAsync()
        {
            var future = AndroidAuth.SignIn();
            return Task.Run(() =>
            {
                var signInResult = (SignInResult)future.Get();
                if (signInResult.Exception != null)
                {
                    throw signInResult.Exception;
                }
                return new UserInformation
                {
                    AccountId = signInResult.UserInformation.AccountId
                };
            });
        }

        private static void PlatformSetConfigUrl(string configUrl)
        {
            AndroidAuth.SetConfigUrl(configUrl);
        }
    }
}
