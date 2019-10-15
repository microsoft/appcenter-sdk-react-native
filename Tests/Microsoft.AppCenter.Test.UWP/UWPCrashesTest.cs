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
        public void VerifyManagedErrorLog()
        {
            try
            {
                throw new System.Exception("test exception");
            }
            catch (System.Exception e)
            {
                // TODO want to check binaries and frames, but this won't be available unless .net native is used.
                // is there a way to check that? Or should it fail unless on .net native?
                ManagedErrorLog managedErrorLog = ErrorLogHelper.CreateErrorLog(e);
                Assert.AreEqual(managedErrorLog.AppLaunchTimestamp.Value.Kind, DateTimeKind.Utc);
#if RELEASE
                Assert.IsNotNull(managedErrorLog.Exception.Frames);
                Assert.IsTrue(managedErrorLog.Exception.Frames.Count > 0);
                Assert.IsNotNull(managedErrorLog.Binaries);
                Assert.IsTrue(managedErrorLog.Binaries.Count > 0);
#endif
            }
        }
    }
}
