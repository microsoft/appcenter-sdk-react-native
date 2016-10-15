using System;

namespace Microsoft.Sonoma.Xamarin.Crashes.Shared
{
    /// <summary>
    /// Interface to abstract <see cref="Crashes"/> features between different platforms.
    /// </summary>
    internal interface IPlatformCrashes
    {
        Type BindingType { get; }

        bool Enabled { get; set; }

        bool HasCrashedInLastSession { get; }

        void GenerateTestCrash();

        void TrackException(Exception exception);
    }
}
