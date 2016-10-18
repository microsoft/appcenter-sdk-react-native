using System;

namespace Microsoft.Sonoma.Crashes.Shared
{
    /// <summary>
    /// Object used to share portable code between platforms.
    /// </summary>
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
