using System;

namespace Microsoft.Sonoma.Xamarin.Crashes.Shared
{
    abstract class PlatformCrashesBase : IPlatformCrashes
    {
        public abstract Type BindingType { get; }

        public abstract bool Enabled { get; set; }

        public abstract bool HasCrashedInLastSession { get; }

        public void GenerateTestCrash()
        {
#if DEBUG
            throw new TestCrashException();
#endif
        }

        public abstract void TrackException(Exception exception);
    }
}
