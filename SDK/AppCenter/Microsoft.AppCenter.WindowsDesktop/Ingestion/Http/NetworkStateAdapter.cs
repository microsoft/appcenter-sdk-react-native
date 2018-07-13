using System;
using System.Net.NetworkInformation;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            NetworkChange.NetworkAddressChanged += (sender, args) => NetworkStatusChanged?.Invoke(sender, args);
            NetworkAbstraction = new NetworkInterfaceAbstraction();
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    return NetworkAbstraction.IsNetworkAvailable();
                }
                catch (Exception e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while checking network state.", e);
                    return false;
                }
            }
        }

        public NetworkInterfaceAbstraction NetworkAbstraction { get; set; }

        public event EventHandler NetworkStatusChanged;

    }
}
