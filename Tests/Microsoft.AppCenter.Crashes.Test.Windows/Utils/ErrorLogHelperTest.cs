// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.AppCenter.Crashes.Test.Windows.Utils
{
    [TestClass]
    public class ErrorLogHelperTest
    {
        [TestInitialize]
        public void SetUp()
        {
            ErrorLogHelper.ProcessInformation = Mock.Of<IProcessInformation>();
            ErrorLogHelper.DeviceInformationHelper = Mock.Of<IDeviceInformationHelper>();
        }

        [TestMethod]
        public void CreateErrorLog()
        {
            // Set up an exception. This is needed because inner exceptions cannot be mocked.
            Exception exception;
            try
            {
                throw new AggregateException("mainException", new Exception("innerException1"), new Exception("innerException2", new Exception("veryInnerException")));
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Mock device information.
            var device = new Microsoft.AppCenter.Ingestion.Models.Device("sdkName", "sdkVersion", "osName", "osVersion", "locale", 1,
                "appVersion", "appBuild", null, null, "model", "oemName", "osBuild", null, "screenSize", null, null, "appNamespace", null, null, null, null);
            Mock.Get(ErrorLogHelper.DeviceInformationHelper).Setup(instance => instance.GetDeviceInformationAsync()).Returns(Task.FromResult(device));

            // Mock process information.
            var parentProcessId = 0;
            var parentProcessName = "parentProcessName";
            var processArchitecture = "processArchitecture";
            var processId = 1;
            var processName = "processName";
            var processStartTime = DateTime.Now;
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ParentProcessId).Returns(parentProcessId);
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ParentProcessName).Returns(parentProcessName);
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ProcessArchitecture).Returns(processArchitecture);
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ProcessId).Returns(processId);
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ProcessName).Returns(processName);
            Mock.Get(ErrorLogHelper.ProcessInformation).SetupGet(instance => instance.ProcessStartTime).Returns(processStartTime);

            // Create the error log.
            var log = ErrorLogHelper.CreateErrorLogAsync(exception).Result;

            // Validate the result.
            Assert.AreEqual(exception.StackTrace, log.Exception.StackTrace);
            Assert.AreEqual(exception.Message, log.Exception.Message);
            Assert.AreEqual(3, log.Exception.InnerExceptions.Count, 3);
            Assert.AreEqual((exception as AggregateException).InnerExceptions[0].Message, log.Exception.InnerExceptions[0].Message);
            Assert.AreEqual((exception as AggregateException).InnerExceptions[1].Message, log.Exception.InnerExceptions[1].Message);
            Assert.AreEqual((exception as AggregateException).InnerExceptions[1].InnerException.Message, log.Exception.InnerExceptions[1].InnerExceptions[0].Message);
            Assert.AreEqual(device.SdkName, log.Device.SdkName);
            Assert.AreEqual(device.SdkVersion, log.Device.SdkVersion);
            Assert.AreEqual(device.OsName, log.Device.OsName);
            Assert.AreEqual(device.OsVersion, log.Device.OsVersion);
            Assert.AreEqual(device.Locale, log.Device.Locale);
            Assert.AreEqual(device.TimeZoneOffset, log.Device.TimeZoneOffset);
            Assert.AreEqual(device.AppVersion, log.Device.AppVersion);
            Assert.AreEqual(device.AppBuild, log.Device.AppBuild);
            Assert.AreEqual(device.WrapperSdkVersion, log.Device.WrapperSdkVersion);
            Assert.AreEqual(device.WrapperSdkName, log.Device.WrapperSdkName);
            Assert.AreEqual(device.Model, log.Device.Model);
            Assert.AreEqual(device.OemName, log.Device.OemName);
            Assert.AreEqual(device.OsBuild, log.Device.OsBuild);
            Assert.AreEqual(device.OsApiLevel, log.Device.OsApiLevel);
            Assert.AreEqual(device.ScreenSize, log.Device.ScreenSize);
            Assert.AreEqual(device.CarrierName, log.Device.CarrierName);
            Assert.AreEqual(device.CarrierCountry, log.Device.CarrierCountry);
            Assert.AreEqual(device.AppNamespace, log.Device.AppNamespace);
            Assert.AreEqual(device.LiveUpdateDeploymentKey, log.Device.LiveUpdateDeploymentKey);
            Assert.AreEqual(device.LiveUpdatePackageHash, log.Device.LiveUpdatePackageHash);
            Assert.AreEqual(device.LiveUpdateReleaseLabel, log.Device.LiveUpdateReleaseLabel);
            Assert.AreEqual(device.WrapperRuntimeVersion, log.Device.WrapperRuntimeVersion);
            Assert.AreEqual(parentProcessId, log.ParentProcessId);
            Assert.AreEqual(parentProcessName, log.ParentProcessName);
            Assert.AreEqual(processArchitecture, log.Architecture);
            Assert.AreEqual(processId, log.ProcessId);
            Assert.AreEqual(processName, log.ProcessName);
            Assert.AreEqual(processStartTime, log.AppLaunchTimestamp);
            Assert.IsTrue(log.Fatal);
        }
    }
}