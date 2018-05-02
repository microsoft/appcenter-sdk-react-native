using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.AppCenter.Crashes
{
    internal static class CrashesUtils
    {
        internal static byte[] SerializeException(Exception exception)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, exception);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(Crashes.LogTag, "Failed to serialize exception for client side inspection", e);
            }
            return null;
        }

        internal static Exception DeserializeException(byte[] exceptionBytes)
        {
            try
            {
                using (var memoryStream = new MemoryStream(exceptionBytes))
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(memoryStream) as Exception;
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(Crashes.LogTag, "Failed to deserialize exception for client side inspection", e);
            }
            return null;
        }
    }
}
