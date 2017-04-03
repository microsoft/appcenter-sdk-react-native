namespace Microsoft.Azure.Mobile.Crashes
{
    internal static class CrashesUtils
    {
        internal static byte[] SerializeException(Exception exception)
        {
            var ms = new MemoryStream();
            var formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(ms, exception);
            }
            catch (SerializationException e)
            {
                MobileCenterLog.Warn(Crashes.LogTag, "Failed to serialize exception for client side inspection", e);
                ms = new MemoryStream();
                formatter.Serialize(ms, e);
            }

            return ms.ToArray();
        }

        internal static Exception DeserializeException(byte[] exceptionBytes)
        {
            var ms = new MemoryStream(exceptionBytes);
            var formatter = new BinaryFormatter();

            Exception deserializedException;

            try
            {
               deserializedException = formatter.Deserialize(ms) as Exception;
            }
            catch(SerializationException e)
            {
                MobileCenterLog.Warn(Crashes.LogTag, "Failed to deserialize exception for client side inspection", e);
                deserializedException = e;
            }

            return deserializedException;
        }
    }
}
