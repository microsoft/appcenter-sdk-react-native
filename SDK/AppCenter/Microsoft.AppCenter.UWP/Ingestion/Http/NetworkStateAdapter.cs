using System;
using Windows.Networking.Connectivity;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        internal delegate bool IsNetworkAvailableDelegate();

        internal IsNetworkAvailableDelegate IsNetworkAvailable { get; set; } = () => NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

        public NetworkStateAdapter()
        {
            NetworkInformation.NetworkStatusChanged += sender => NetworkStatusChanged?.Invoke(sender, EventArgs.Empty);
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    return IsNetworkAvailable();
                }
                catch (Exception e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while checking network state.", e);
                    return false;
                }
            }
        }

        public event EventHandler NetworkStatusChanged;
    }
}
