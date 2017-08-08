using System;
using Foundation;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorReport
    {
        internal ErrorReport(MSErrorReport msReport)
        {
            // If Id is not null we have loaded the report from the cache
            if (Id != null)
            {
                return;
            }

            Id = msReport.IncidentIdentifier;
            AppStartTime = NSDateToDateTimeOffset(msReport.AppStartTime);
            AppErrorTime = NSDateToDateTimeOffset(msReport.AppErrorTime);
            Device = msReport.Device == null ? null : new Device(msReport.Device);

            AndroidDetails = null;

            iOSDetails = new iOSErrorDetails(msReport.ReporterKey,
                                             msReport.Signal,
                                             msReport.ExceptionName,
                                             msReport.ExceptionReason,
                                             (uint)msReport.AppProcessIdentifier);

            MSWrapperException wrapperException = MSWrapperExceptionManager.LoadWrapperExceptionWithUUID(msReport.IncidentIdentifier);
            if (wrapperException != null && wrapperException.ExceptionData != null)
            {
                Exception = CrashesUtils.DeserializeException(wrapperException.ExceptionData.ToArray());
            }
        }

        private DateTimeOffset NSDateToDateTimeOffset(NSDate date)
        {
            DateTime dateTime = (DateTime)date;
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime;
        }
    }
}
