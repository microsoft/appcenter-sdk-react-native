using System;
using System.Net.NetworkInformation;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        private event Action PrivateNetworkAddressChanged;

        public NetworkStateAdapter()
        {
            NetworkChange.NetworkAddressChanged += (sender, args) => PrivateNetworkAddressChanged?.Invoke();
        }
        public bool IsConnected => NetworkInterface.GetIsNetworkAvailable();

        public event Action NetworkAddressChanged
        {
            add => PrivateNetworkAddressChanged += value;
            remove => PrivateNetworkAddressChanged -= value;
        }
    }
}
