using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP
{
    /*
     * These are stubs for methods that do not belong here and do not have an implementation yet.
     * They may not even have proper type signatures yet. Provided for temporary use in other classes.
     */
    public static class MiscStubs
    {
        public static bool IsRecoverableHttpError(Exception e)
        {
            return true;
        }

        public static Ingestion.Models.Device GetDeviceInfo()
        {
            return new Ingestion.Models.Device();
        }

        public static long CurrentTimeInMilliseconds()
        {
           return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        //adapted from https://msdn.microsoft.com/en-us/library/bb412179(v=vs.110).aspx
        public static string WriteFromObject<T>(T item) //TODO might not work with an interface
        {
            //TODO using stuff is a bit weird and unnecessary
            string jsonString = "";
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(ms, item);
                byte[] json = ms.ToArray();
                jsonString = Encoding.UTF8.GetString(json, 0, json.Length);
            }
            return jsonString;
        }

        //adapted from https://msdn.microsoft.com/en-us/library/bb412179(v=vs.110).aspx
        public static T ReadToObject<T>(string json)
        {
            //TODO using stuff is a bit weird and unnecessary
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
