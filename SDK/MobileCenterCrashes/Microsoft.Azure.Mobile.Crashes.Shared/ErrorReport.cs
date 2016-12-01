using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorReport
    {
        public string Id { get; }

        public DateTimeOffset AppStartTime { get; }

        public DateTimeOffset AppErrorTime { get; }

        public Device Device { get; }

        public Exception Exception { get; }

        public AndroidErrorDetails AndroidDetails { get; }

        public iOSErrorDetails iOSDetails { get; }

        private static Dictionary<string, ErrorReport> cachedReports = new Dictionary<string, ErrorReport>();

        private ErrorReport(ErrorReport other)
        {
            if (other == null)
            {
                return;
            }
            Id = other.Id;
            AppStartTime = other.AppStartTime;
            AppErrorTime = other.AppErrorTime;
            Device = other.Device;
            Exception = other.Exception;
            AndroidDetails = other.AndroidDetails;
            iOSDetails = other.iOSDetails;
        }
    }
}
