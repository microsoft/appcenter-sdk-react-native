// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [TestClass]
    public class EventLogTest
    {
        private readonly DateTime? Timestamp = null;
        private const string Name = "Name";
        private readonly Guid Id = Guid.Empty;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            var mockDevice = new Mock<Device>();

            var emptyLog = new EventLog();
            var log = new EventLog(Timestamp, mockDevice.Object, Id, Name);

            Assert.IsNotNull(emptyLog);
            Assert.IsNotNull(log);

            Assert.AreEqual(Id, log.Id);
            Assert.AreEqual(Name, log.Name);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when Name == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenNameIsNull()
        {
            const string NullName = null;
            var mockDevice = new Mock<Device>();

            var log = new EventLog(Timestamp, mockDevice.Object, Id, NullName);
            Assert.ThrowsException<ValidationException>(() => log.Validate());
        }
    }
}
