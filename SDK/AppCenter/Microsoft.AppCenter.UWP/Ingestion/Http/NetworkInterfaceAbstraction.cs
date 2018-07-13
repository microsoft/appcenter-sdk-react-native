using Windows.Networking.Connectivity;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkInterfaceAbstraction
    {
        public bool IsNetworkAvailable()
        {
            return NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}