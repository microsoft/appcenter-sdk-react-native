using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Analytics
{
    /// <summary>
    ///     Analytics feature.
    /// </summary>
    public class Analytics : IMobileCenterService
    {
        internal Analytics()
        {
        }

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
        }

        ///// <summary>
        ///// Enable or disable automatic page tracking.
        ///// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        ///// </summary>
        //public static bool AutoPageTrackingEnabled { get; set; }

        /// <summary>
        ///     Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, IDictionary<string, string> properties = null)
        {
        }

        ///// Track a custom page.

        ///// <summary>
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //    throw new NotImplementedException();
        //}
    }
}