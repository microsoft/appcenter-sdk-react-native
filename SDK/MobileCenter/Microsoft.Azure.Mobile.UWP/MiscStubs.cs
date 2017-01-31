using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP
{
    /*
     * These are stubs for methods that do not belong here and do not have an implementation yet.
     * They may not even have proper type signatures yet. Provided for temporary use in other classes.
     */
    public static class MiscStubs
    {
        public static bool IsRecoverableHttpError(Exception e)
        {
            return true;
        }

        public static Ingestion.Models.Device GetDeviceInfo()
        {
            return new Ingestion.Models.Device();
        }

        public static long CurrentTimeInMilliseconds()
        {
           return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
