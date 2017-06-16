using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Distribute
{
    public static partial class Distribute
    {
        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        static void PlatformSetEnabled(bool enabled)
        {
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
        }

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
        }
    }
}
