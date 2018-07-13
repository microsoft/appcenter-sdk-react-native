using System;
using Windows.Networking.Connectivity;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            NetworkInformation.NetworkStatusChanged += sender => NetworkStatusChanged?.Invoke(sender, EventArgs.Empty);
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

        public event EventHandler NetworkStatusChanged;
        public NetworkInterfaceAbstraction NetworkAbstraction { get; set; }
    }
}
