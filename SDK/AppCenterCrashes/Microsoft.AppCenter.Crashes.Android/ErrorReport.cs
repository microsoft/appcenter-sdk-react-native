// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Crashes
{
    using AndroidExceptionDataManager = Com.Microsoft.Appcenter.Crashes.WrapperSdkExceptionManager;
    using AndroidErrorReport = Com.Microsoft.Appcenter.Crashes.Model.AndroidErrorReport;

    public partial class ErrorReport
    {
        internal ErrorReport(AndroidErrorReport androidReport)
        {
            Id = androidReport.Id;
            AppStartTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppStartTime.Time);
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppErrorTime.Time);
            Device = androidReport.Device == null ? null : new Device(androidReport.Device);
            var androidStackTrace = androidReport.StackTrace;
            AndroidDetails = new AndroidErrorDetails(androidStackTrace, androidReport.ThreadName);
            iOSDetails = null;
            string exceptionString = AndroidExceptionDataManager.LoadWrapperExceptionData(Java.Util.UUID.FromString(Id));
            if (exceptionString != null)
            {
                StackTrace = exceptionString;
            }
        }
    }
}
