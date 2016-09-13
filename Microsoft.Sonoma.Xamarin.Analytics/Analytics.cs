using System;
using System.Collections.Generic;

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

        public static void TrackEvent(string name, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public static void TrackPage(string name, IDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }
    }
}
