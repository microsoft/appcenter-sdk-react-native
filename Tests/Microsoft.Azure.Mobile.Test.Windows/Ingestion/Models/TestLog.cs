using System;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Mobile.Ingestion.Models.Device;

    class TestLog: Log
    {
        public TestLog() {}
        public TestLog(DateTime? timestamp, Device device, System.Guid? sid = default(System.Guid?))
            : base(timestamp, device, sid){ }
    }
}
