using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Sonoma.Xamarin.Analytics
{
    public static class Analytics
    {
        public static bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public static bool AutoPageTrackingEnabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }
}
