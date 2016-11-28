using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidExceptionDataManager = Com.Microsoft.Azure.Mobile.Crashes.WrapperSdkExceptionManager;
    using AndroidErrorReport = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidErrorReport;

    public partial class ErrorReport
    {
        internal ErrorReport(AndroidErrorReport androidReport)
        {
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
                    SystemException = CrashesUtils.DeserializeException(exceptionBytes);
                }
            }
            AndroidDetails = new AndroidErrorDetails(androidThrowable, androidReport.ThreadName);
            iOSDetails = null;
        }
    }
}
