using System;
using Microsoft.Sonoma.Xamarin.Crashes.Shared;

namespace Microsoft.Sonoma.Xamarin.Crashes
{
    public static class Crashes
    {
        private static readonly IPlatformCrashes PlatformCrashes = new PlatformCrashes();

        public static Type BindingType => PlatformCrashes.BindingType;

        public static bool Enabled
        {
            get { return PlatformCrashes.Enabled; }
            set { PlatformCrashes.Enabled = value; }
        }

        public static bool HasCrashedInLastSession => PlatformCrashes.HasCrashedInLastSession;

        public static void GenerateTestCrash()
        {
            PlatformCrashes.GenerateTestCrash();
        }

        public static void TrackException(Exception exception)
        {
            PlatformCrashes.TrackException(exception);
        }
    }
}
