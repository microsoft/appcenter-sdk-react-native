// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils.Fakes;
using Castle.DynamicProxy.Generators;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class CrashesTest
    {
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;
        private Mock<ErrorLogHelper> _mockErrorLogHelper;

        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockErrorLogHelper = new Mock<ErrorLogHelper>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            //ApplicationLifecycleHelper.Instance = _applicationLifecycleHelper;
        }

        [TestMethod]
        public void InstanceIsNotNull()
        {
            Crashes.Instance = null;
            Assert.IsNotNull(Crashes.Instance);
        }

        [TestMethod]
        public void GetEnabled()
        {
            Crashes.SetEnabledAsync(false).Wait();
            Assert.IsFalse(Crashes.IsEnabledAsync().Result);

            Crashes.SetEnabledAsync(true).Wait();
            Assert.IsTrue(Crashes.IsEnabledAsync().Result);
        }

        [TestMethod]
        public void ApplyEnabledStateStartsListening()
        {
            // enabled state is applied
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            bool passed = false;
            IStorageOperationsHelper storageOperationsHelper = new StubIStorageOperationsHelper()
            {
                SaveErrorLogFileManagedErrorLog = (managedErrorLog) => 
                {
                    passed = true;
                }
            };

            // TODO Raise an arbitrary event for UnhandledExceptionOccurred handler

            _mockChannel.Verify(channel => channel.SetEnabled(true), Times.Once());
            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ApplyEnabledStateCleansUp()
        {
            // disabled state is applied
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannel.Verify(channel => channel.SetEnabled(false), Times.Once());
            // listener stops listening
            //_applicationLifecycleHelper.InvokeUnhandledException();

            // listener is listening
            _mockErrorLogHelper.Verify(errorLogHelper => errorLogHelper.SaveErrorLogFile(It.IsAny<ManagedErrorLog>()), Times.Never());
            
            // error log files are deleted
            _mockErrorLogHelper.Verify(errorLogHelper => ErrorLogHelper.RemoveStoredErrorLogFile(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void ApplyEnabledStateDeleteFails()
        {
            // disabled state is applied
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // file deletion throws
            
        }
    }
}
