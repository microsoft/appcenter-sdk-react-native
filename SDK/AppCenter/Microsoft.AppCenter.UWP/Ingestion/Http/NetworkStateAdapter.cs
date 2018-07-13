using System;
using Windows.Networking.Connectivity;

namespace Microsoft.AppCenter.Ingestion.Http
{
    /// <summary>
    /// Network state adapter.
    /// </summary>
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        internal delegate bool IsNetworkAvailableDelegate();

        internal IsNetworkAvailableDelegate IsNetworkAvailable { private get; set; } = CheckNetworkAvailable;

        /// <summary>
        /// Init.
        /// </summary>
        public NetworkStateAdapter()
        {
            NetworkInformation.NetworkStatusChanged += sender => NetworkStatusChanged?.Invoke(sender, EventArgs.Empty);
        }

        private static bool CheckNetworkAvailable()
        {
            return NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }

        /// <summary>
        /// Check if network is connected.
        /// </summary>
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

        /// <summary>
        /// Event to subscribe to network status changes.
        /// </summary>
        public event EventHandler NetworkStatusChanged;
    }
}
