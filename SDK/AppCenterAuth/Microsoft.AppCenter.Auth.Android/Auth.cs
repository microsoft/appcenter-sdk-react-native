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
                var userInformation = (AndroidUserInformation)future.Get();
                return new UserInformation
                {
                    AccountId = userInformation.AccountId
                };
            });
        }
    }
}
