using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    class TestLog: Log
    {
        public TestLog() {}
        public TestLog(long toffset, Device device, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid){ }
    }
}
