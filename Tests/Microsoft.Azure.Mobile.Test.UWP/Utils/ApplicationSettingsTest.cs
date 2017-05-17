using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Storage;

namespace Microsoft.Azure.Mobile.Test.UWP.Utils
{
    [TestClass]
    public class ApplicationSettingsTest
    {
        private readonly IApplicationSettings settings = new ApplicationSettings();

        [TestInitialize]
        public void InitializeMobileCenterTest()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();
        }

        /// <summary>
        /// Verify access to values
        /// </summary>
        [TestMethod]
        public void VerifyAccess()
        {
            const string key = "test";
            Assert.AreEqual(null, settings[key]);
            settings[key] = "test";
            Assert.AreEqual("test", settings[key]);
            settings[key] = 42;
            Assert.AreEqual(42, settings[key]);
        }

        /// <summary>
        /// Verify GetValue generic method behaviour
        /// </summary>
        [TestMethod]
        public void VerifyGetValue()
        {
            const string key = "test";
            Assert.AreEqual(null, settings[key]);
            Assert.AreEqual(42, settings.GetValue(key, 42));
            Assert.AreEqual(42, settings[key]);
            Assert.AreEqual(42, settings.GetValue(key, 0));
        }

        /// <summary>
        /// Verify remove values from settings
        /// </summary>
        [TestMethod]
        public void VerifyRemove()
        {
            const string key = "test";
            Assert.AreEqual(null, settings[key]);
            settings[key] = 42;
            Assert.AreEqual(42, settings[key]);
            settings.Remove(key);
            Assert.AreEqual(null, settings[key]);
        }
    }
}
