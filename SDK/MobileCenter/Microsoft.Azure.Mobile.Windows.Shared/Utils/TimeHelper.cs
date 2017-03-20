using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public static class TimeHelper
    {
        public static long CurrentTimeInMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
