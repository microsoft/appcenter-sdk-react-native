using System;

namespace Microsoft.Sonoma.Xamarin.Crashes.Shared
{
    internal interface IPlatformCrashes
    {
        Type BindingType { get; }

        bool Enabled { get; set; }

        bool HasCrashedInLastSession { get; }

        void GenerateTestCrash();

        void TrackException(Exception exception);
    }
}
