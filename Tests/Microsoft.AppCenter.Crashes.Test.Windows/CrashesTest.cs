// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class CrashesTest
    {
        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
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
        public void LastSessionErrorReportIsNullOnStart()
        {
            Assert.IsNull(Crashes.GetLastSessionCrashReportAsync().Result);
            Assert.IsFalse(Crashes.HasCrashedInLastSessionAsync().Result);
        }
    }
}
