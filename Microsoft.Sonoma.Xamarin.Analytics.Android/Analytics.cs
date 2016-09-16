using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Microsoft.Sonoma.Xamarin.Analytics
{
    using AndroidAnalytics = Com.Microsoft.Sonoma.Analytics.Analytics;

    public static class Analytics
    {
        public static Type BindingType => typeof(AndroidAnalytics);

        public static bool Enabled
        {
            get { return AndroidAnalytics.Enabled; }
            set { AndroidAnalytics.Enabled = value; }
        }

        public static bool AutoPageTrackingEnabled
        {
            get { return AndroidAnalytics.AutoPageTrackingEnabled; }
            set { AndroidAnalytics.AutoPageTrackingEnabled = value; }
        }

        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackEvent(name, properties);
        }

        public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackPage(name, properties);
        }
    }
}
