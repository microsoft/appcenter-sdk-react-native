using System.Collections.Generic;
using Microsoft.Azure.Mobile.Crashes.iOS.Bindings;

namespace Microsoft.Azure.Mobile.Crashes
{
    static class ErrorReportCache
    {
        readonly static Dictionary<string, ErrorReport> cachedReports = new Dictionary<string, ErrorReport>();

        internal static ErrorReport GetErrorReport(MSErrorReport msReport)
        {
            ErrorReport cachedReport;
            if (cachedReports.TryGetValue(msReport.IncidentIdentifier, out cachedReport))
            {
                return cachedReport;
            }

            var newErrorReport = new ErrorReport(msReport);
            cachedReports[msReport.IncidentIdentifier] = newErrorReport;
            return newErrorReport;
        }
    }
}
