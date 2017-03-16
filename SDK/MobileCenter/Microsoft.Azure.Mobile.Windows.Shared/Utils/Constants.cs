using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Utils
{
    public static class Constants
    {
        /* Channel constants */
        public  const int DefaultTriggerCount = 50;
        public static readonly TimeSpan DefaultTriggerInterval = TimeSpan.FromSeconds(3);
        public const int DefaultTriggerMaxParallelRequests = 3;
    }
}
