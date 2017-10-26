using Android.Runtime;

namespace Microsoft.AppCenter.Distribute
{
    [Preserve]
    class DownloadManagerReceiver : Com.Microsoft.AppCenter.Distribute.DownloadManagerReceiver
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
