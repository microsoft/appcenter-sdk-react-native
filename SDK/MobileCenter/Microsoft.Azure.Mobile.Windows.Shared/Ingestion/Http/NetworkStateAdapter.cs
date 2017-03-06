using System.Net.NetworkInformation;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public interface INetworkStateAdapter
    {
        bool IsConnected { get; }

        event NetworkAddressChangedEventHandler NetworkAddressChanged;
    }

    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public bool IsConnected => NetworkInterface.GetIsNetworkAvailable();

        public event NetworkAddressChangedEventHandler NetworkAddressChanged
        {
            add { NetworkChange.NetworkAddressChanged += value; }
            remove { NetworkChange.NetworkAddressChanged -= value; }
        }
    }
}
