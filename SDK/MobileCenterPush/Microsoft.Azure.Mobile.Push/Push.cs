using System.Threading.Tasks;

namespace Microsoft.AppCenter.Push
{
    public partial class Push
    {
        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }
    }
}
