using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;

namespace Microsoft.Azure.Mobile.Analytics
{
    using System.Linq;
    using iOSAnalytics = iOS.Bindings.MSAnalytics;

    /// <summary>
    /// Analytics feature.
    /// </summary>
    public class Analytics : MobileCenterService
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(iOSAnalytics);

        /// <summary>
        /// Enable or disable Analytics module.
        /// </summary>
        public static bool Enabled
        {
            get { return iOSAnalytics.IsEnabled(); }
            set { iOSAnalytics.SetEnabled(value); }
        }

        /// <summary>
        /// Enable or disable automatic page tracking.
        /// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        /// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //	get { return iOSAnalytics.IsAutoPageTrackingEnabled(); }
        //	set { iOSAnalytics.SetAutoPageTrackingEnabled(value); }
        //}

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            if (properties != null)
            {
                iOSAnalytics.TrackEvent(name, StringDictToNSDict(properties));
                return;
            }
            iOSAnalytics.TrackEvent(name);
        }

        ///// <summary>
        ///// Track a custom page.
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //	if (properties != null)
        //	{
        //		iOSAnalytics.TrackPage(name, StringDictToNSDict(properties));
        //		return;
        //	}
        //	iOSAnalytics.TrackPage(name);
        //}

        private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }
    }
}
