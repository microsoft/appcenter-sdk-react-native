using System;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// Various constants used by the SDK.
    /// </summary>
    public static class Constants
    {
        // Prefix for Mobile Center application settings
        public const string KeyPrefix = "MobileCenter";

        // Channel constants
        public const int DefaultTriggerCount = 50;
        public static readonly TimeSpan DefaultTriggerInterval = TimeSpan.FromSeconds(3);
        public const int DefaultTriggerMaxParallelRequests = 3;
    }
}
