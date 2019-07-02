using System;
using System.IO;
using SystemBinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace Microsoft.AppCenter.Crashes.Utils
{
    internal class BinaryFormatter
    {
        private readonly SystemBinaryFormatter _systemBinaryFormatter = new SystemBinaryFormatter();

        public virtual void Serialize(Stream stream, Exception exception)
        {
            _systemBinaryFormatter.Serialize(stream, exception);
        }

        public virtual Exception Deserialize(Stream stream)
        {
            return _systemBinaryFormatter.Deserialize(stream) as Exception;
        }
    }
}
