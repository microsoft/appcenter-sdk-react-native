using System.Diagnostics;

namespace Microsoft.AppCenter
{
    public partial class AppCenter
    {
        private const string PlatformIdentifier = "windowsdesktop";

        static void PlatformSetUserId(string userId)
        {
            Debug.WriteLine("This API is not supported on Windows.");
        }
    }
}
