using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Sonoma.Analytics
{
    using AndroidAnalytics = Com.Microsoft.Sonoma.Analytics.Analytics;

    /// <summary>
    /// Analytics feature.
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Analytics bindings type.
        /// </value>
        public static Type BindingType => typeof(AndroidAnalytics);

        /// <summary>
        /// Enable or disable Analytics module.
        /// </summary>
        public static bool Enabled
        {
            get { return AndroidAnalytics.Enabled; }
            set { AndroidAnalytics.Enabled = value; }
        }

        /// <summary>
        /// Enable or disable automatic page tracking.
        /// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        /// </summary>
        public static bool AutoPageTrackingEnabled
        {
            get { return AndroidAnalytics.AutoPageTrackingEnabled; }
            set { AndroidAnalytics.AutoPageTrackingEnabled = value; }
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackEvent(name, properties);
        }

        /// <summary>
        /// Track a custom page.
        /// </summary>
        /// <param name="name">A page name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackPage(name, properties);
        }
    }
}
