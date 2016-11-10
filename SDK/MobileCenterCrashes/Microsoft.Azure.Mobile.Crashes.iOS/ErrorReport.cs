using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public partial class ErrorReport
    {
        internal ErrorReport(MSErrorReport msReport)
        {
            Id = msReport.IncidentIdentifier;
            //AppStartTime = msReport.AppStartTime //TODO convert
            //AppStartTime = msReport.AppErrorTime //TODO convert
            Device = msReport.Device == null ? null : new Device(msReport.Device);
            //TODO others        
        }
    }
}
