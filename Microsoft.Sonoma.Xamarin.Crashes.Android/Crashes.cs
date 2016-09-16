using System;

namespace Microsoft.Sonoma.Xamarin.Crashes
{
    using AndroidCrashes = Com.Microsoft.Sonoma.Crashes.Crashes;

    public static class Crashes
    {
        public static Type BindingType => typeof(AndroidCrashes);

        public static bool Enabled
        {
            get { return AndroidCrashes.Enabled; }
            set { AndroidCrashes.Enabled = value; }
        }
    }
}