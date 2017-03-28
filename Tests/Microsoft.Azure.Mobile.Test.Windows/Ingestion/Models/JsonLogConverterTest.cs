using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    class TestJsonReader : JsonReader
    {
        public override bool Read()
        {
            return true;
        }
    }

    [TestClass]
    public class JsonLogConverterTest
    {
        private LogJsonConverter _converter;
        private const string TestType = "testType";

        [TestInitialize]
        public void InitializeJsonLogConverterTest()
        {
            _converter = new LogJsonConverter();
        }

        /// <summary>
        /// Validate that conveter throws exception for null type
        /// </summary>
        [TestMethod]
        public void ReadJsonThrowExceptionForNullTypeName()
        {
            JObject jObj = new JObject();
            Assert.ThrowsException<JsonReaderException>(() => _converter.ReadJson(jObj.CreateReader(), typeof(JObject), null, null));
        }

        /// <summary>
        /// Validate that conveter throws exception for unexpected type
        /// </summary>
        [TestMethod]
        public void ReadJsonThrowsExceptionForUnexpectedTypeName()
        {
            JObject jObj = CreateJObjectWithType(TestType);
            Assert.ThrowsException<JsonReaderException>(() => _converter.ReadJson(jObj.CreateReader(), typeof(JObject), null, null));
        }

        /// <summary>
        /// Validate that ReadJson doesn't throw exception for expected type name
        /// </summary>
        [TestMethod]
        public void ReadJsonDoesntThrowExceptionForExpectedTypeName()
        {
            JObject jObj = CreateJObjectWithType(TestType);
            _converter.AddLogType(TestType, typeof(JObject));
            var readResult = _converter.ReadJson(jObj.CreateReader(), typeof(JObject), null, null);
            Assert.IsNotNull(readResult);
        }

        private JObject CreateJObjectWithType(string type)
        {
            JObject jObj = new JObject();
            jObj.Add(LogSerializer.TypeIdKey, TestType);
            return jObj;
        }
    }
}
