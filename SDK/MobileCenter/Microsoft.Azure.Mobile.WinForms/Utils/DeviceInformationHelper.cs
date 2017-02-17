using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Utils
{
    class DeviceInformationHelper : IDeviceInformationHelper
    {
        public Ingestion.Models.Device GetDeviceInformation()
        {
            throw new NotImplementedException();
        }

        public event Action InformationInvalidated;
    }
}
