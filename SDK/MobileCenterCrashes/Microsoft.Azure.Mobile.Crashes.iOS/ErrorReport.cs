using System;
using Foundation;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public partial class ErrorReport
    {
        internal ErrorReport(MSErrorReport msReport)
        {
            Id = msReport.IncidentIdentifier;
            AppStartTime = NSDateToDateTimeOffset(msReport.AppStartTime);
            AppStartTime = NSDateToDateTimeOffset(msReport.AppErrorTime);
            Device = msReport.Device == null ? null : new Device(msReport.Device);

            AndroidDetails = null;

            iOSDetails = new iOSErrorDetails(msReport.ReporterKey, 
                                             msReport.Signal, 
                                             msReport.ExceptionName, 
                                             msReport.ExceptionReason, 
                                             (uint)msReport.AppProcessIdentifier);

            NSData wrapperExceptionData = MSWrapperExceptionManager.LoadWrapperExceptionData(msReport.IncidentIdentifier);
            SystemException = DeserializeException(wrapperExceptionData);
        }

        private DateTimeOffset NSDateToDateTimeOffset(NSDate date)
        {
            DateTime dateTime = (DateTime)date;
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime;
        }

        private Exception DeserializeException(NSData data)
        {
            byte[] exceptionBytes = data.ToArray();
            MemoryStream ms = new MemoryStream(exceptionBytes);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(ms) as Exception;
        }
    }
}
