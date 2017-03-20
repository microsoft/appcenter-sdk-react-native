using System;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public bool IsConnected { get; }
        public event Action NetworkAddressChanged;
    }
}
