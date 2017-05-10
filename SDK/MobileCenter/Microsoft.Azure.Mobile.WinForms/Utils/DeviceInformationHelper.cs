using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : IDeviceInformationHelper
    {
        public static event EventHandler InformationInvalidated;

        public Ingestion.Models.Device GetDeviceInformation()
        {
            throw new NotImplementedException();
        }
    }
}
