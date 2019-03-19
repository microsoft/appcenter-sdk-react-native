// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable RECS0154 // Parameter is never used: portable methods are stubs so every method will trigger this.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes
    {
        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        private static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            return Task.FromResult(false);
        }

        private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.FromResult((ErrorReport)null);
        }

        private static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
        }

        private static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
        }
    }
}
