using System;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Various constants used by the SDK.
    /// </summary>
    public static class Constants
    {
        // Prefix for App Center application settings
        public const string KeyPrefix = "AppCenter";

        // Channel constants
        public const int DefaultTriggerCount = 50;
        public static readonly TimeSpan DefaultTriggerInterval = TimeSpan.FromSeconds(3);
        public const int DefaultTriggerMaxParallelRequests = 3;
    }
}
