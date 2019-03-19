// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Rum;

namespace Microsoft.AppCenter.Rum
{
    public partial class RealUserMeasurements
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidRealUserMeasurements);

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidRealUserMeasurements.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidRealUserMeasurements.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static void PlatformSetRumKey(string rumKey)
        {
            AndroidRealUserMeasurements.SetRumKey(rumKey);
        }
    }
}
