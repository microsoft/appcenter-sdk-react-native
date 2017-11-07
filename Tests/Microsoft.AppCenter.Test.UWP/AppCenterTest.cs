using System;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Core;

namespace Microsoft.AppCenter.Test.UWP
{
    [TestClass]
    public class AppCenterTest
    {
        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            AppCenter.Instance = null;
        }

        /// <summary>
        /// Verify configure with UWP platform id
        /// </summary>
        [TestMethod]
        public void VerifyPlatformId()
        {
            CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppCenter.Configure("uwp=appsecret");
            }).AsTask().GetAwaiter().GetResult();

            Assert.IsTrue(AppCenter.Configured);
        }

        /// <summary>
        /// Verify country code setter
        /// </summary>
        [TestMethod]
        public void SetCountryCode()
        {
            var informationInvalidated = false;

            void InformationInvalidated(object sender, EventArgs e)
            {
                informationInvalidated = true;
            }

            DeviceInformationHelper.InformationInvalidated += InformationInvalidated;
            AppCenter.SetCountryCode("US");
            DeviceInformationHelper.InformationInvalidated -= InformationInvalidated;
            Assert.AreEqual(informationInvalidated, true);
        }
    }
}
