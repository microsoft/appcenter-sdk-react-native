/*
 * IMPORTANT: This file is not a local copy. It is located in Microsoft.Azure.Mobile.Crashes.Shared. To use it, add a 
 * link to this file to the desired project. It cannot be added to any project that supports Silverlight. Silverlight
 * does not contain BinaryFormatter.
 */

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.Azure.Mobile.Crashes
{
    public static class CrashesUtils
    {
        public static byte[] SerializeException(Exception exception)
        {
            var ms = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, exception);
            return ms.ToArray();
        }

        public static Exception DeserializeException(byte[] exceptionBytes)
        {
            var ms = new MemoryStream(exceptionBytes);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(ms) as Exception;
        }
    }
}