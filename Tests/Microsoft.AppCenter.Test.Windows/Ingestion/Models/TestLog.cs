// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    class TestLog: Log
    {
        public TestLog() {}
        public TestLog(DateTime? timestamp, Device device, System.Guid? sid = default(System.Guid?))
            : base(device, timestamp, sid) { }
    }
}
