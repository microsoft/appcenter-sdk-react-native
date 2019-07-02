using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Microsoft.AppCenter.Utils.Files;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
