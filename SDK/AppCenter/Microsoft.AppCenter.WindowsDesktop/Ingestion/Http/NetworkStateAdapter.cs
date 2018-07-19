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

        public bool IsConnected
        {
            get
            {
                try
                {
                    return NetworkInterface.GetIsNetworkAvailable();
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
