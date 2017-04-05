using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Azure.Mobile.Test.Windows.Utils
{
    [TestClass]
    public class UnhandledExceptionEventArgsTest
    {
        /// <summary>
        /// Validate that Unhandled exception args saves correct exception
        /// </summary>
        [TestMethod]
        public void CheckExceptionReference()
        {
            Exception ex = new Exception();
            UnhandledExceptionOccurredEventArgs args =
                new UnhandledExceptionOccurredEventArgs(ex);
            Assert.AreSame(ex, args.Exception);
        }
    }
}
