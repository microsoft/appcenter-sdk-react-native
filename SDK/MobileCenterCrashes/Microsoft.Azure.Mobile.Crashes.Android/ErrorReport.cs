using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidExceptionDataManager = Com.Microsoft.Azure.Mobile.Crashes.WrapperSdkExceptionManager;
    using AndroidErrorReport = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorReport;

    public partial class ErrorReport
    {
        internal ErrorReport(AndroidErrorReport androidReport) : this(CachedReportIfExists(androidReport))
        {
            // If Id is not null we have loaded the report from the cache
            if (Id != null)
            {
                return;
            }

            Id = androidReport.Id;
            AppStartTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppStartTime.Time);
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppErrorTime.Time);
            Device = androidReport.Device == null ? null : new Device(androidReport.Device);

            object androidThrowable;
            try
            {
                androidThrowable = androidReport.Throwable;
            }
            catch (Exception e)
            {
                MobileCenterLog.Debug(Crashes.LogTag, "Cannot read throwable from java point of view, probably a .NET exception", e);
                androidThrowable = null;
                byte[] exceptionBytes = AndroidExceptionDataManager.LoadWrapperExceptionData(Java.Util.UUID.FromString(Id));
                if (exceptionBytes != null)
                {
                    Exception = CrashesUtils.DeserializeException(exceptionBytes);
                }
            }

            AndroidDetails = new AndroidErrorDetails(androidThrowable, androidReport.ThreadName);
            iOSDetails = null;

            cachedReports[Id] = this;
        }

        static private ErrorReport CachedReportIfExists(AndroidErrorReport androidReport)
        {
            ErrorReport cachedReport;
            if (cachedReports.TryGetValue(androidReport.Id, out cachedReport))
            {
                return cachedReport;
            }
            return null;
        }
    }
}
