using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Mobile.Crashes;

namespace Microsoft.Azure.Mobile.Test.UWP
{
    [TestClass]
    public class CorrelationIdTest
    {
        [TestInitialize]
        public void InitializeTests()
        {
            MobileCenter.Start("secret", typeof(Crashes.Crashes));
        }

        /// <summary>
        /// Verify that crashes sets the correlation id on start.
        /// </summary>
        [TestMethod]
        public void CrashesSetsCorrelationId()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            Assert.IsNotNull(MobileCenter.CorrelationId);
#pragma warning restore CS0612 // Type or member is obsolete
        }

        // Verify that crashes replaces a null correlation id.
        [TestMethod]
        public void CrashesSetsCorrelationIdWhenMadeNull()
        {
#pragma warning disable CS0612 // Type or member is obsolete
            MobileCenter.CorrelationId = null;
            Assert.IsNotNull(MobileCenter.CorrelationId);
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }
}
