// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Push.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [TestClass]
    public class PushInstallationLogTest
    {
        private static readonly DateTime? Timestamp = null;

        [TestMethod]
        public void TestConstructor()
        {
            var mockDevice = new Mock<Device>();

            PushInstallationLog log = new PushInstallationLog(mockDevice.Object, "token1", Timestamp, default(System.Guid?), "userId1");
            Assert.IsNotNull(log);
            Assert.AreEqual(default(System.Guid?), log.Sid);
            Assert.AreEqual("token1", log.PushToken);
            Assert.AreEqual("userId1", log.UserId);

            PushInstallationLog log2 = new PushInstallationLog(mockDevice.Object, "token2", Timestamp, System.Guid.NewGuid(), "userId2");
            Assert.IsNotNull(log2);
            Assert.IsNotNull(log2.Sid);
            Assert.AreEqual("token2", log2.PushToken);
            Assert.AreEqual("userId2", log2.UserId);
        }

        [TestMethod]
        public void TestValidateThrowsExceptionWhenPushTokenIsNull()
        {
            var mockDevice = new Mock<Device>();

            PushInstallationLog log = new PushInstallationLog(mockDevice.Object, null, Timestamp, null, null);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
