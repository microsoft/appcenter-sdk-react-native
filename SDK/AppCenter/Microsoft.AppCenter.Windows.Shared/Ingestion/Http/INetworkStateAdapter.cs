using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public interface INetworkStateAdapter
    {
        bool IsConnected { get; }

        event EventHandler NetworkStatusChanged;
    }
}
 