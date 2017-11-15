using System;
using System.Net.NetworkInformation;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            NetworkChange.NetworkAddressChanged += (sender, args) => NetworkStatusChanged?.Invoke(sender, args);
        }
        public bool IsConnected => NetworkInterface.GetIsNetworkAvailable();
        public event EventHandler NetworkStatusChanged;
    }
}
