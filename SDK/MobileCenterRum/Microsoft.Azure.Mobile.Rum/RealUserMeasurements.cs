using System.Threading.Tasks;

namespace Microsoft.AppCenter.Rum
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
    }
}
