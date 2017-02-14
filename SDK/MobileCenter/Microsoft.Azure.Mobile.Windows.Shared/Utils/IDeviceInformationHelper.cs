using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public interface IDeviceInformationHelper
    {
        Ingestion.Models.Device GetDeviceInformation();
        Task<Ingestion.Models.Device> GetDeviceInformationAsync();

    }
}
