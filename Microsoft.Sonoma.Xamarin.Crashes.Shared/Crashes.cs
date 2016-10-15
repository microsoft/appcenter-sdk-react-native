using System;
using Microsoft.Sonoma.Xamarin.Crashes.Shared;

namespace Microsoft.Sonoma.Xamarin.Crashes
{
    /// <summary>
    /// Crashes feature.
    /// </summary>
    public static class Crashes
    {
        private static readonly IPlatformCrashes PlatformCrashes = new PlatformCrashes();

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The target SDK Crashes bindings type.
        /// </value>
        public static Type BindingType => PlatformCrashes.BindingType;

        /// <summary>
        /// Enable or disable Crashes module.
        /// </summary>
        public static bool Enabled
        {
            get { return PlatformCrashes.Enabled; }
            set { PlatformCrashes.Enabled = value; }
        }

        /// <summary>
        /// Provides information whether the app crashed in its last session.
        /// </summary>
        /// <value>
        /// <c>true</c> if a crash was recorded in the last session, otherwise <c>false</c>.
        /// </value>
        public static bool HasCrashedInLastSession => PlatformCrashes.HasCrashedInLastSession;

        /// <summary>
        /// Generates crash for test purpose.
        /// </summary>
        /// <remarks>
        /// This call has no effect in non debug configuration (such as release).
        /// </remarks>
        public static void GenerateTestCrash()
        {
            PlatformCrashes.GenerateTestCrash();
        }

        /// <summary>
        /// Track an exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        public static void TrackException(Exception exception)
        {
            PlatformCrashes.TrackException(exception);
        }
    }
}
