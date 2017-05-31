using Windows.ApplicationModel.Activation;

namespace Microsoft.Azure.Mobile
{
    public abstract partial class MobileCenterService
    {
        public virtual void NotifyOnLaunched(LaunchActivatedEventArgs e)
        {
            // Do nothing by default but can be overridden
        }
    }
}
