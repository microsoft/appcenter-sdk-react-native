using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Data.Xml.Dom;

namespace Microsoft.Azure.Mobile.Test.UWP
{
    [TestClass]
    public class UWPPushTest
    {
        /// <summary>
        /// Verify ParseLaunchString works when launch string is null
        /// </summary>
        [TestMethod]
        public void ParseLaunchStringWhenStringIsNull()
        {
            var result = Push.Push.ParseLaunchString(null);
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verify ParseLaunchString works when launch string is empty
        /// </summary>
        [TestMethod]
        public void ParseLaunchStringWhenStringIsEmpty()
        {
            var result = Push.Push.ParseLaunchString(string.Empty);
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verify ParseLaunchString works when launch string is not valid Json
        /// </summary>
        [TestMethod]
        public void ParseLaunchStringWhenStringContainsInvalidJson()
        {
            var actualResult = Push.Push.ParseLaunchString("{\"mobile_center\":{\"key1\":\"value1\",\"key2\":\"value2\"");
            Assert.IsNull(actualResult);
        }

        /// <summary>
        /// Verify ParseLaunchString works when launch string doesn't contain "mobile_center"
        /// </summary>
        [TestMethod]
        public void ParseLaunchStringWhenStringDoesNotContainMobileCenter()
        {
            var actualResult = Push.Push.ParseLaunchString("{\"foobar\":{\"key1\":\"value1\",\"key2\":\"value2\"}}");
            Assert.IsNull(actualResult);
        }

        /// <summary>
        /// Verify ParseLaunchString works
        /// </summary>
        [TestMethod]
        public void ParseLaunchString()
        {
            var actualResult = Push.Push.ParseLaunchString("{\"mobile_center\":{\"key1\":\"value1\",\"key2\":\"value2\"}}");
            var expectedResult = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };

            Assert.AreEqual(2, actualResult.Count);
            Assert.AreEqual(expectedResult["key1"], actualResult["key1"]);
            Assert.AreEqual(expectedResult["key2"], actualResult["key2"]);
        }

        /// <summary>
        /// Verify ParseMobileCenterPush works when custom data is null
        /// </summary>
        [TestMethod]
        public void ParseMobileCenterPushWhenCustomDataIsNull()
        {
            var xmlContent = new XmlDocument();
            xmlContent.LoadXml("<?xml version=\"1.0\" encoding=\"utf-16\"?><toast><visual><binding template=\"ToastImageAndText02\"><text id=\"1\">test-title</text><text id=\"2\">hello world</text></binding></visual></toast>");
            var actualResult = Push.Push.ParseMobileCenterPush(xmlContent);
            Assert.IsNull(actualResult);
        }

        /// <summary>
        /// Verify ParseMobileCenterPush works
        /// </summary>
        [TestMethod]
        public void ParseMobileCenterPush()
        {
            var xmlContent = new XmlDocument();

            xmlContent.LoadXml("<?xml version=\"1.0\" encoding=\"utf-16\"?><toast launch=\"{&quot;mobile_center&quot;:{&quot;key1&quot;:&quot;value1&quot;,&quot;key2&quot;:&quot;value2&quot;}}\"><visual><binding template=\"ToastImageAndText02\"><text id=\"1\">test-title</text><text id=\"2\">hello world</text></binding></visual></toast>");
            var actualResult = Push.Push.ParseMobileCenterPush(xmlContent);

            var expectedResult = new Push.PushNotificationReceivedEventArgs
            {
                Title = "test-title",
                Message = "hello world",
                CustomData = new Dictionary<string, string>
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2"
                }
            };

            Assert.AreEqual(expectedResult.Title, actualResult.Title);
            Assert.AreEqual(expectedResult.Message, actualResult.Message);
            Assert.AreEqual(expectedResult.CustomData["key1"], actualResult.CustomData["key1"]);
            Assert.AreEqual(expectedResult.CustomData["key2"], actualResult.CustomData["key2"]);
        }
    }
}
