// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public interface INetworkStateAdapter
    {
        bool IsConnected { get; }

        event EventHandler NetworkStatusChanged;
    }
}
 
