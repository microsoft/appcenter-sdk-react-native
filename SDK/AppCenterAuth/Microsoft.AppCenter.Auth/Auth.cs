// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth
    {
        private static Task<UserInformation> PlatformSignInAsync()
        {
            var task = new TaskCompletionSource<UserInformation>();
            task.SetException(new NotImplementedException());
            return task.Task;
        }
    }
}
