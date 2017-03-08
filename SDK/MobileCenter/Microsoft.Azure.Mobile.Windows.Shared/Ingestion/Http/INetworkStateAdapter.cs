using System;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public interface INetworkStateAdapter
    {
        bool IsConnected { get; }

        event Action NetworkAddressChanged;
    }
}

 