using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidErrorReport = Com.Microsoft.Azure.Mobile.Crashes.Model.ErrorReport;

    public partial class ErrorReport
    {
        internal ErrorReport(AndroidErrorReport androidReport)
        {
            Id = androidReport.Id;
            AppStartTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppStartTime.Time);
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppErrorTime.Time);
            Device = androidReport.Device == null ? null : new Device(androidReport.Device);

            iOSDetails = null;
            //FIXME: The error is here! when accessing androidReport.Throwable, we seem to crash...
            AndroidDetails = new AndroidErrorDetails(androidReport.Throwable, androidReport.ThreadName);

        }
    }
}
