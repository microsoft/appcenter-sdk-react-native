using System.Net.NetworkInformation;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkInterfaceAbstraction
    {
        public bool IsNetworkAvailable() 
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}