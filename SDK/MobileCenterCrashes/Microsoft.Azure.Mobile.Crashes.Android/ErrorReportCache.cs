using System.Collections.Generic;
using Com.Microsoft.Azure.Mobile.Crashes.Model;

namespace Microsoft.Azure.Mobile.Crashes
{
    static class ErrorReportCache
    {
        readonly static Dictionary<string, ErrorReport> cachedReports = new Dictionary<string, ErrorReport>();

        internal static ErrorReport GetErrorReport(AndroidErrorReport androidReport)
        {
            ErrorReport cachedReport;
            if (cachedReports.TryGetValue(androidReport.Id, out cachedReport))
            {
                return cachedReport;
            }

            var newErrorReport = new ErrorReport(androidReport);
            cachedReports[androidReport.Id] = newErrorReport;
            return newErrorReport;
        }
    }
}
