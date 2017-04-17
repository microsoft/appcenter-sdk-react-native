using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows
{
    [TestClass]
    public class CustomPropertiesTest
    {
        /// <summary>
        /// Verify that key validated correct.
        /// </summary>
        [TestMethod]
        public void TestKeyValidate()
        {
            var value1 = "test";
            var value2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var value3 = 1;
            var value4 = 0.1f;
            var value5 = false;
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);

            /* Null key. */
            string nullKey = null;
            properties.Set(nullKey, value1);
            properties.Set(nullKey, value2);
            properties.Set(nullKey, value3);
            properties.Set(nullKey, value4);
            properties.Set(nullKey, value5);
            properties.Clear(nullKey);
            Assert.AreEqual(0, properties.Properties.Count);

            /* Invalid key. */
            var invalidKey = "!";
            properties.Set(invalidKey, value1);
            properties.Set(invalidKey, value2);
            properties.Set(invalidKey, value3);
            properties.Set(invalidKey, value4);
            properties.Set(invalidKey, value5);
            properties.Clear(invalidKey);
            Assert.AreEqual(0, properties.Properties.Count);

            /* Normal keys. */
            properties.Set("t1", value1);
            properties.Set("t2", value2);
            properties.Set("t3", value3);
            properties.Set("t4", value4);
            properties.Set("t5", value5);
            properties.Clear("t6");
            Assert.AreEqual(6, properties.Properties.Count);

            /* Already contains keys. */
            properties.Set("t1", value1);
            properties.Set("t2", value2);
            properties.Set("t3", value3);
            properties.Set("t4", value4);
            properties.Set("t5", value5);
            properties.Clear("t6");
            Assert.AreEqual(6, properties.Properties.Count);
        }

        /// <summary>
        /// Verify that string setting correct.
        /// </summary>
        [TestMethod]
        public void TestSetString()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);

            /* Null value. */
            string nullValue = null;
            properties.Set(key, nullValue);
            Assert.AreEqual(0, properties.Properties.Count);

            /* Normal value. */
            var normalValue = "test";
            properties.Set(key, normalValue);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.AreEqual(normalValue, properties.Properties[key]);
        }

        /// <summary>
        /// Verify that date setting correct.
        /// </summary>
        [TestMethod]
        public void TestSetDate()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);
            
            /* Normal value. */
            var normalValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            properties.Set(key, normalValue);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.AreEqual(normalValue, properties.Properties[key]);
        }

        /// <summary>
        /// Verify that int setting correct.
        /// </summary>
        [TestMethod]
        public void TestSetInteger()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);

            /* Normal value. */
            var normalValue = 1;
            properties.Set(key, normalValue);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.AreEqual(normalValue, properties.Properties[key]);
        }

        /// <summary>
        /// Verify that float setting correct.
        /// </summary>
        [TestMethod]
        public void TestSetFloat()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);

            /* Normal value. */
            var normalValue = 0.1f;
            properties.Set(key, normalValue);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.AreEqual(normalValue, properties.Properties[key]);
        }

        /// <summary>
        /// Verify that bool setting correct.
        /// </summary>
        [TestMethod]
        public void TestSetBool()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);

            /* Normal value. */
            var normalValue = false;
            properties.Set(key, normalValue);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.AreEqual(normalValue, properties.Properties[key]);
        }

        /// <summary>
        /// Verify that clear correct.
        /// </summary>
        [TestMethod]
        public void TestClear()
        {
            var key = "test";
            CustomProperties properties = new CustomProperties();
            Assert.AreEqual(0, properties.Properties.Count);
            properties.Clear(key);
            Assert.AreEqual(1, properties.Properties.Count);
            Assert.IsNull(properties.Properties[key]);
        }
    }
}
