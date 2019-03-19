// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Ingestion.Http;

namespace Microsoft.AppCenter.Channel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret, INetworkStateAdapter networkState);
    }
}
