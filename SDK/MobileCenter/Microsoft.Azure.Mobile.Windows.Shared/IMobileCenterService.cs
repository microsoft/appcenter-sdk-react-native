using Microsoft.Azure.Mobile.Channel;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// Represents a module that provides a service through Mobile Center.
    /// </summary>
    public interface IMobileCenterService
    {
        /// <summary>
        /// Display name of the service
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets or sets whether the service is enabled
        /// </summary>
        bool InstanceEnabled { get; set; }

        /// <summary>
        /// Method that is called to signal start of service.
        /// </summary>
        /// <param name="channelGroup">The channel group to which the channel should be added</param>
        void OnChannelGroupReady(IChannelGroup channelGroup);
    }
}
