// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Utils
{
    [TestClass]
    public class DeviceInformationHelperTest
    {
        /// <summary>
        /// Verify sdk name in device information
        /// </summary>
        [TestMethod]
        public void VerifySdkName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.SdkName, "appcenter.winforms");
        }

        /// <summary>
        /// Verify device oem name in device information
        /// </summary>
        [TestMethod]
        public void VerifyDeviceOemName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreNotEqual(device.OemName.ToLower(), "system manufacturer");
        }

        /// <summary>
        /// Verify device model in device model.
        /// </summary>
        [TestMethod]
        public void VerifyDeviceModel()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreNotEqual(device.Model.ToLower(), "system product name");
        }
    }
}
