using System;
using Microsoft.Azure.Mobile;

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
            //TODO others
        }
    }
}
