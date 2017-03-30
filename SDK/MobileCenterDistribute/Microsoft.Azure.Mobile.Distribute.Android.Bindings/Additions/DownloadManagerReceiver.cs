using Android.Runtime;

namespace Microsoft.Azure.Mobile.Distribute
{
    [Preserve]
    class DownloadManagerReceiver : Com.Microsoft.Azure.Mobile.Distribute.DownloadManagerReceiver
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
