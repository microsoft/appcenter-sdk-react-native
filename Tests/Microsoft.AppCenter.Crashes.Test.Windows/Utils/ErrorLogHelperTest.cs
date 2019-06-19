// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Fakes;
using System.Linq;
using System.Security;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            System.Exception exception;
            try
            {
                throw new AggregateException("mainException", new System.Exception("innerException1"), new System.Exception("innerException2", new System.Exception("veryInnerException")));
            }
            catch (System.Exception e)
            {
                exception = e;
            }

            // Mock device information.
            var device = new Microsoft.AppCenter.Ingestion.Models.Device("sdkName", "sdkVersion", "osName", "osVersion", "locale", 1,
                "appVersion", "appBuild", null, null, "model", "oemName", "osBuild", null, "screenSize", null, null, "appNamespace", null, null, null, null);
            Mock.Get(ErrorLogHelper.DeviceInformationHelper).Setup(instance => instance.GetDeviceInformation()).Returns(device);

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
            var log = ErrorLogHelper.CreateErrorLog(exception);

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

        [TestMethod]
        public void GetSingleErrorLogFile()
        {
            var id = Guid.NewGuid();
            var expectedFileInfo = new FileInfo("file");
            var fileInfoList = new List<FileInfo> { expectedFileInfo };
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == $"{id}.json" ? fileInfoList : null;
                };

                // Retrieve the error log by the ID.
                var errorLogFileInfo = ErrorLogHelper.GetStoredErrorLogFile(id);

                // Validate the contents.
                Assert.AreSame(expectedFileInfo, errorLogFileInfo);
            }
        }

        [TestMethod]
        [DataRow(typeof(DirectoryNotFoundException))]
        [DataRow(typeof(SecurityException))]
        public void GetSingleErrorLogFileDoesNotThrow(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    throw exception;
                };

                // Retrieve the error log by the ID.
                var errorLogFileInfo = ErrorLogHelper.GetStoredErrorLogFile(Guid.NewGuid());
                Assert.IsNull(errorLogFileInfo);
            }
        }

        [TestMethod]
        public void GetErrorLogFiles()
        {
            // Mock multiple error log files.
            var expectedFileInfo1 = new FileInfo("file");
            var expectedFileInfo2 = new FileInfo("file2");
            var fileInfoList = new List<FileInfo> { expectedFileInfo1, expectedFileInfo2 };
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == "*.json" ? fileInfoList : null;
                };

                // Retrieve the error logs.
                var errorLogFileInfos = ErrorLogHelper.GetErrorLogFiles().ToList();

                // Validate the contents.
                Assert.AreEqual(fileInfoList.Count, errorLogFileInfos.Count);
                foreach (var fileInfo in errorLogFileInfos)
                {
                    Assert.IsNotNull(fileInfo);
                    CollectionAssert.Contains(fileInfoList, fileInfo);
                    fileInfoList.Remove(fileInfo);
                }
            }
        }

        [TestMethod]
        [DataRow(typeof(DirectoryNotFoundException))]
        [DataRow(typeof(SecurityException))]
        public void GetErrorLogFilesDoesNotThrow(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    throw exception;
                };
                var errorLogFiles = ErrorLogHelper.GetErrorLogFiles();
                Assert.IsNull(errorLogFiles);
            }
        }

        [TestMethod]
        public void GetLastErrorLogFile()
        {
            using (ShimsContext.Create())
            {
                // Mock multiple error log files.
                var oldFileInfo = new ShimFileInfo();
                var oldFileSystemInfo = new ShimFileSystemInfo(oldFileInfo)
                {
                    LastWriteTimeGet = () => DateTime.Now.AddDays(-200)
                };
                var recentFileInfo = new ShimFileInfo();
                var recentFileSystemInfo = new ShimFileSystemInfo(recentFileInfo)
                {
                    LastWriteTimeGet = () => DateTime.Now
                };
                var fileInfoList = new List<FileInfo> { oldFileInfo, recentFileInfo };
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == "*.json" ? fileInfoList : null;
                };

                // Retrieve the error logs.
                var errorLogFileInfo = ErrorLogHelper.GetLastErrorLogFile();

                // Validate the contents.
                Assert.AreSame(recentFileInfo.Instance, errorLogFileInfo);
            }
        }

        [TestMethod]
        [DataRow(typeof(DirectoryNotFoundException))]
        [DataRow(typeof(SecurityException))]
        public void GetLastErrorLogFileDoesNotThrow(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    throw exception;
                };

                // Retrieve the error logs.
                var errorLogFileInfo = ErrorLogHelper.GetLastErrorLogFile();
                Assert.IsNull(errorLogFileInfo);
            }
        }

        [TestMethod]
        [DataRow(typeof(IOException))]
        [DataRow(typeof(PlatformNotSupportedException))]
        [DataRow(typeof(ArgumentOutOfRangeException))]
        public void GetLastErrorLogFileDoesNotThrowWhenLastWriteTimeThrows(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            using (ShimsContext.Create())
            {
                // Mock multiple error log files.
                var oldFileInfo = new ShimFileInfo();
                var oldFileSystemInfo = new ShimFileSystemInfo(oldFileInfo)
                {
                    LastWriteTimeGet = () => { throw exception; }
                };
                var recentFileInfo = new ShimFileInfo();
                var recentFileSystemInfo = new ShimFileSystemInfo(oldFileInfo)
                {
                    LastWriteTimeGet = () => { throw exception; }
                };
                var fileInfoList = new List<FileInfo> { oldFileInfo, recentFileInfo };
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == "*.json" ? fileInfoList : null;
                };

                // Retrieve the error logs.
                var errorLogFileInfo = ErrorLogHelper.GetLastErrorLogFile();
                Assert.IsNull(errorLogFileInfo);
            }
        }

        [TestMethod]
        public void GetLastErrorLogFileWhenOnlyOneIsSaved()
        {
            using (ShimsContext.Create())
            {
                // Mock multiple error log files.
                var fileInfo = new ShimFileInfo();
                var fileSystemInfo = new ShimFileSystemInfo(fileInfo)
                {
                    LastWriteTimeGet = () => DateTime.Now.AddDays(-200)
                };
                var fileInfoList = new List<FileInfo> { fileInfo };
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == "*.json" ? fileInfoList : null;
                };

                // Retrieve the error logs.
                var errorLogFileInfo = ErrorLogHelper.GetLastErrorLogFile();

                // Validate the contents.
                Assert.AreSame(fileInfo.Instance, errorLogFileInfo);
            }
        }

        [TestMethod]
        public void SaveErrorLogFile()
        {
            var errorLog = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                ProcessId = 123
            };
            var filePath = Path.Combine(Constants.AppCenterFilesDirectoryLocation, Constants.AppCenterFilesDirectoryName, "Errors", errorLog.Id + ".json");
            var serializedErrorLog = LogSerializer.Serialize(errorLog);
            using (ShimsContext.Create())
            {
                var actualPath = "";
                var actualContents = "";
                var callCount = 0;
                ShimDirectoryInfo.AllInstances.ExistsGet = info => true;
                ShimFile.WriteAllTextStringString = (path, contents) =>
                {
                    callCount++;
                    actualPath = path;
                    actualContents = contents;
                };
                ErrorLogHelper.SaveErrorLogFile(errorLog);
                Assert.AreEqual(filePath, actualPath);
                Assert.AreEqual(serializedErrorLog, actualContents);
                Assert.AreEqual(1, callCount);
            }
        }

        [TestMethod]
        [DataRow(typeof(ArgumentException))]
        [DataRow(typeof(ArgumentNullException))]
        [DataRow(typeof(PathTooLongException))]
        [DataRow(typeof(DirectoryNotFoundException))]
        [DataRow(typeof(IOException))]
        [DataRow(typeof(UnauthorizedAccessException))]
        [DataRow(typeof(NotSupportedException))]
        [DataRow(typeof(SecurityException))]
        public void SaveErrorLogFileDoesNotThrow(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            var errorLog = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                ProcessId = 123
            };
            var fileName = errorLog.Id + ".json";
            var serializedErrorLog = LogSerializer.Serialize(errorLog);
            using (ShimsContext.Create())
            {
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    throw exception;
                };
                ErrorLogHelper.SaveErrorLogFile(errorLog);
            }
            // No exception should be thrown.
        }

        [TestMethod]
        public void RemoveStoredErrorLogFile()
        {
            using (ShimsContext.Create())
            {
                var fileInfo = new ShimFileInfo();
                var count = 0;
                fileInfo.Delete = () => { count++; };
                var fileInfoList = new List<FileInfo> { fileInfo };
                var id = Guid.NewGuid();
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == $"{id}.json" ? fileInfoList : null;
                };
                ErrorLogHelper.RemoveStoredErrorLogFile(id);
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        [DataRow(typeof(IOException))]
        [DataRow(typeof(SecurityException))]
        [DataRow(typeof(UnauthorizedAccessException))]
        public void RemoveStoredErrorLogFileDoesNotThrow(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;
            using (ShimsContext.Create())
            {
                var fileInfo = new ShimFileInfo
                {
                    Delete = () => { throw exception; }
                };
                var fileInfoList = new List<FileInfo> { fileInfo };
                var id = Guid.NewGuid();
                ShimDirectoryInfo.AllInstances.EnumerateFilesString = (info, pattern) =>
                {
                    return pattern == $"{id}.json" ? fileInfoList : null;
                };
                ErrorLogHelper.RemoveStoredErrorLogFile(id);
            }
            // No exception should be thrown.
        }
    }
}
