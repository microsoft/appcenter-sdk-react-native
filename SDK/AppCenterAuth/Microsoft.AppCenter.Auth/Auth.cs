// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth
    {
        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        private static Task<UserInformation> PlatformSignInAsync()
        {
            var task = new TaskCompletionSource<UserInformation>();
            task.SetException(new NotImplementedException());
            return task.Task;
        }

        private static void PlatformSignOut()
        {
        }
    }
}
