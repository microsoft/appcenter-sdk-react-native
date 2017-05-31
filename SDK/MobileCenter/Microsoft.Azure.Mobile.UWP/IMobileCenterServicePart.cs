using Windows.ApplicationModel.Activation;

namespace Microsoft.Azure.Mobile
{
    public partial interface IMobileCenterService
    {
        /// <summary>
        /// Signals application launch.
        /// </summary>
        /// <param name="e">The event args passed to the OnLaunched method</param>
        void NotifyOnLaunched(LaunchActivatedEventArgs e);
    }
}
