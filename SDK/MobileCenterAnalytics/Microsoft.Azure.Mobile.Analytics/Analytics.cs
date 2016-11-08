using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Mobile.Analytics
{
    /// <summary>
    /// Analytics feature.
    /// </summary>
    public static class Analytics
    {
        /// <summary>
        /// Enable or disable Analytics module.
        /// </summary>
        public static bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        ///// <summary>
        ///// Enable or disable automatic page tracking.
        ///// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        ///// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //    get { throw new NotImplementedException(); }
        //    set { throw new NotImplementedException(); }
        //}

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Track a custom page.
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
