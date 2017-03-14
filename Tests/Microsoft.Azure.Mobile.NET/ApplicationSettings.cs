using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    /*
     * Application settings implemented in-memory with no persistence
     */

    [ExcludeFromCodeCoverage]
    public class ApplicationSettings : IApplicationSettings
    {
        private static readonly Dictionary<object, object> Settings = new Dictionary<object, object>();

        public object this[string key]
        {
            get { return Settings[key]; }
            set { Settings[key] = value; }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            var found = Settings.TryGetValue(key, out result);
            if (found)
            {
                return (T) result;
            }
            this[key] = defaultValue;
            return defaultValue;
        }

        public void Remove(string key)
        {
            Settings.Remove(key);
        }

        public static void Reset()
        {
            Settings.Clear();
        }

        public static bool IsEmpty => Settings.Count == 0;
    }
}
