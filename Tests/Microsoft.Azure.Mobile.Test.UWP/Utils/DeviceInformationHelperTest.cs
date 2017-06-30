using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Test.UWP.Utils
{
    [TestClass]
    public class DeviceInformationHelperTest
    {
        /// <summary>
        /// Verify sdk name in device information
        /// </summary>
        [TestMethod]
        public void VerifySdkName()
        {
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.SdkName, "mobilecenter.uwp");
        }

        /// <summary>
        /// Verify carrier country in device information
        /// </summary>
        [TestMethod]
        public void VerifyCarrierCountry()
        {
            const string CountryCode = "US";
            MobileCenter.SetCountryCode(CountryCode);

            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.CarrierCountry, CountryCode);
        }

        /// <summary>
        /// Verify screen size provider
        /// </summary>
        [TestMethod]
        public void VerifyScreenSizeProvider()
        {
            const string testScreenSize = "screen_size";
            var informationInvalidated = false;
            var screenSizeProviderMock = new Mock<IScreenSizeProvider>();
            var screenSizeProviderFactoryMock = new Mock<IScreenSizeProviderFactory>();

            screenSizeProviderMock.Setup(provider => provider.ScreenSize).Returns(testScreenSize);
            screenSizeProviderFactoryMock.Setup(factory => factory.CreateScreenSizeProvider()).Returns(screenSizeProviderMock.Object);
            DeviceInformationHelper.SetScreenSizeProviderFactory(screenSizeProviderFactoryMock.Object);

            // Screen size is returned from screen size provider
            var device = Task.Run(() => new DeviceInformationHelper().GetDeviceInformationAsync()).Result;
            Assert.AreEqual(device.ScreenSize, testScreenSize);

            // InformationInvalidated is invoked when ScreenSizeChanged event is raised
            DeviceInformationHelper.InformationInvalidated += (sender, args) => { informationInvalidated = true; };
            screenSizeProviderMock.Raise(provider => provider.ScreenSizeChanged += null, EventArgs.Empty);
            Assert.IsTrue(informationInvalidated);
        }
    }
}
