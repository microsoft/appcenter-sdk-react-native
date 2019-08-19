// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AppCenter.Utils;
using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Utils
{
    public partial class DeviceInformationHelperTest
    {
        /// <summary>
        /// Verify sdk name in device information
        /// </summary>
        [Fact]
        public void VerifySdkName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.Equal("appcenter.wpf.netcore", device.SdkName);
        }
    }
}
