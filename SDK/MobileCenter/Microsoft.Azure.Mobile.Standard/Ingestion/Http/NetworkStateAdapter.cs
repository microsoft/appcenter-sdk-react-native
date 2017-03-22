using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        private static readonly TimeSpan PollTimeSpan = TimeSpan.FromMilliseconds(100);
        private static bool _shouldMonitor;

        static NetworkStateAdapter()
        {
            BeginMonitoringNetworkStatus();
        }

        public NetworkStateAdapter()
        {
            NetworkConnectionChangedShared += NetworkAddressChanged;
        }

        public bool IsConnected
        {
            get
            {
                if (!_hasValidStatus)
                {
                    _isConnected = GetConnectedStatus();
                    _hasValidStatus = true;
                }
                return _isConnected;
            }
        }

        private static bool _isConnected;
        private static bool _hasValidStatus;


        private static void BeginMonitoringNetworkStatus()
        {
            _shouldMonitor = true;
            Task.Run(() =>
            {
                while (true)
                {
                    if (!_shouldMonitor)
                    {
                        return;
                    }
                    if (_isConnected != GetConnectedStatus())
                    {
                        _isConnected = !_isConnected;
                        NetworkConnectionChangedShared?.Invoke(null, EventArgs.Empty);
                    }
                    System.Threading.Thread.Sleep(PollTimeSpan);
                }
            });
        }

        private static void StopMonitoringNetworkStatus()
        {
            _shouldMonitor = false;
        }

        private static event EventHandler NetworkConnectionChangedShared;

        private static bool GetConnectedStatus()
        {
            try
            {
                var hostName = Dns.GetHostName();
                var hostEntryTask = Dns.GetHostEntryAsync(hostName);
                hostEntryTask.Wait();
                if (hostEntryTask.Result.AddressList.Length == 0)
                {
                    return false;
                }
                var ipAddress = hostEntryTask.Result.AddressList[0];
                return !IPAddress.IsLoopback(ipAddress);
            }
            catch
            {
                return false;
            }
        }

        public event EventHandler NetworkAddressChanged;
    }
}



