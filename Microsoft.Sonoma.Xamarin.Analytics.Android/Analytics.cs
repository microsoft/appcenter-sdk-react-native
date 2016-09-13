using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.Sonoma.Xamarin.Analytics
{
    using AndroidAnalytics = Com.Microsoft.Sonoma.Analytics.Analytics;

    public static class Analytics
    {
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

        public static void TrackEvent(string name, IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackEvent(name, properties);
        }

        public static void TrackPage(string name, IDictionary<string, string> properties)
        {
            AndroidAnalytics.TrackPage(name, properties);
        }

        public static Type GetBindingType()
        {
            return typeof (AndroidAnalytics);
        }
    }
}
