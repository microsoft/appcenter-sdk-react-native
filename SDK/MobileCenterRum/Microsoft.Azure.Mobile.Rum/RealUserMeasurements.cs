using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Rum
{
    public partial class RealUserMeasurements
    {
        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        static void PlatformSetRumKey(string rumKey)
        {
        }

        static void PlatformSetConfigurationUrl(string url)
        {
        }
    }
}
