using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.AppCenter.Test.UWP
{
    [TestClass]
    public class UWPCrashesTest
    {
        [TestMethod]
        public void VerifyAppLaunchTimestampUTC()
        {
            ManagedErrorLog managedErrorLog = ErrorLogHelper.CreateErrorLog(new System.Exception("Test Exception"));
            Assert.AreEqual(managedErrorLog.AppLaunchTimestamp.Value.Kind, DateTimeKind.Utc);
        }
    }
}
