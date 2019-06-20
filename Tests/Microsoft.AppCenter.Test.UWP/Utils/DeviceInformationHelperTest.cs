// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.UWP.Utils
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
            Assert.AreEqual(device.SdkName, "appcenter.uwp");
        }

        /// <summary>
        /// Verify carrier country in device information
        /// </summary>
        [TestMethod]
        public void VerifyCarrierCountry()
        {
            const string CountryCode = "US";
            AppCenter.SetCountryCode(CountryCode);

            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.CarrierCountry, CountryCode);
        }
    }
}
