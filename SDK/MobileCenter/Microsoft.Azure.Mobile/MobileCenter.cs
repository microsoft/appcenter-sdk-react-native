#define DEBUG

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile
{
    public partial class MobileCenter
    {
        internal MobileCenter()
        {
        }

        /* Error message to display for unsupported targets. */
        const string ErrorMessage =
            "[MobileCenter] ASSERT: Cannot use Mobile Center on this target. If you are on Android or iOS or UWP, you must add the NuGet packages in the Android and iOS and UWP projects as well. Other targets are not yet supported.";

        static LogLevel PlatformLogLevel { get; set; }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        static void PlatformSetEnabled(bool enabled)
        {
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            return Task.FromResult((Guid?)null);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
        }

        static bool PlatformConfigured { get; }

        static void PlatformConfigure(string appSecret)
        {
            Debug.WriteLine(ErrorMessage);
        }

        static void PlatformStart(params Type[] services)
        {
            Debug.WriteLine(ErrorMessage);
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            Debug.WriteLine(ErrorMessage);
        }
    }
}