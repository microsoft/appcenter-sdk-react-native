using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorReport
    {
        public string Id { get; }

        public DateTimeOffset AppStartTime { get; }

        public DateTimeOffset AppErrorTime { get; }

        public Device Device { get; }

        public AndroidErrorDetails AndroidDetails { get; }

        public iOSErrorDetails iOSDetails { get; }
    }
}
