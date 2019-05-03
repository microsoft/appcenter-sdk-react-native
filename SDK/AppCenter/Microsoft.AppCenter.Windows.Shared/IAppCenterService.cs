// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Channel;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// Represents a module that provides a service through App Center.
    /// </summary>
    public interface IAppCenterService
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
        /// <param name="appSecret">The app secret of the current application</param>

        void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret);
    }
}
