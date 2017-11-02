using Android.Runtime;

namespace Microsoft.AppCenter.Distribute
{
    [Preserve]
    class DeepLinkActivity : Com.Microsoft.Appcenter.Distribute.DeepLinkActivity
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
