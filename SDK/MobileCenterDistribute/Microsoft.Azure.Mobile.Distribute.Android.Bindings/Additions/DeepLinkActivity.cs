using Android.Runtime;

namespace Microsoft.Azure.Mobile.Distribute
{
    [Preserve]
    class DeepLinkActivity : Com.Microsoft.Azure.Mobile.Distribute.DeepLinkActivity
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
