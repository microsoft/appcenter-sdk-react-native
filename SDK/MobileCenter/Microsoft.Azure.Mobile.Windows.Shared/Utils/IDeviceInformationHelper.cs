using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public interface IDeviceInformationHelper
    {
        Ingestion.Models.Device GetDeviceInformation();
        event Action InformationInvalidated;
    }
}
