// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Analytics;

namespace Microsoft.AppCenter.Analytics
{
    /// <summary>
    /// Analytics service.
    /// </summary>
    public class Analytics : AppCenterService
    {
        internal Analytics()
        {
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidAnalytics);

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            var future = AndroidAnalytics.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            var future = AndroidAnalytics.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        ///// <summary>
        ///// Enable or disable automatic page tracking.
        ///// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        ///// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //    get { return AndroidAnalytics.AutoPageTrackingEnabled; }
        //    set { AndroidAnalytics.AutoPageTrackingEnabled = value; }
        //}

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, IDictionary<string, string> properties = null)
        {
            AndroidAnalytics.TrackEvent(name, properties);
        }

        ///// <summary>
        ///// Track a custom page.
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //    AndroidAnalytics.TrackPage(name, properties);
        //}

        internal static void UnsetInstance()
        {
            AndroidAnalytics.UnsetInstance();
        }
    }
}
