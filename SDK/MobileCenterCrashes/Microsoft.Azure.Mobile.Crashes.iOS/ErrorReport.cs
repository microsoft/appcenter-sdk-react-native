using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public partial class ErrorReport
    {
        internal ErrorReport(MSErrorReport msReport) : this(CachedReportIfExists(msReport))
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

            NSData wrapperExceptionData = MSWrapperExceptionManager.LoadWrapperExceptionData(msReport.IncidentIdentifier);
            if (wrapperExceptionData != null)
            {
                Exception = CrashesUtils.DeserializeException(wrapperExceptionData.ToArray());
            }

            cachedReports[Id] = this;
        }

        private DateTimeOffset NSDateToDateTimeOffset(NSDate date)
        {
            DateTime dateTime = (DateTime)date;
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime;
        }

        static private ErrorReport CachedReportIfExists(MSErrorReport msReport)
        {
            ErrorReport cachedReport;
            if (cachedReports.TryGetValue(msReport.IncidentIdentifier, out cachedReport))
            {
                return cachedReport;
            }
            return null;
        }
    }
}
