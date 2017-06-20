using System;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            NetworkInformation.NetworkStatusChanged += sender => NetworkStatusChanged?.Invoke(sender, EventArgs.Empty);
        }

        public bool IsConnected
        {
            get
            {
                return NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;          
            }
        }
        public event EventHandler NetworkStatusChanged;
    }
}
