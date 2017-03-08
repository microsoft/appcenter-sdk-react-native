using System;
using System.Net.NetworkInformation;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            NetworkChange.NetworkAddressChanged += (sender, args) => NetworkAddressChanged?.Invoke();
        }
        public bool IsConnected => NetworkInterface.GetIsNetworkAvailable();
        public event Action NetworkAddressChanged;
    }
}
