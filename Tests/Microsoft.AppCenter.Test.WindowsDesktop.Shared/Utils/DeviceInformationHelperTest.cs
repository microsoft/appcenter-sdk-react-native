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
        /// Verify device oem name in device information
        /// </summary>
        [Fact]
        public void VerifyDeviceOemName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.NotEqual(device.OemName.ToLower(), AbstractDeviceInformationHelper.DefaultSystemManufacturer);
        }

        /// <summary>
        /// Verify device model in device model.
        /// </summary>
        [Fact]
        public void VerifyDeviceModel()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.NotEqual(device.Model.ToLower(), AbstractDeviceInformationHelper.DefaultSystemProductName);
        }
    }
}
