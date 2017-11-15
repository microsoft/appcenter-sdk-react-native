using System;
using Windows.Networking.Connectivity;

namespace Microsoft.AppCenter.Ingestion.Http
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
