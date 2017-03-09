using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    /*
     * Application settings implemented in-memory with no persistence
     */
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly Dictionary<object, object> _settings = new Dictionary<object, object>();

        public object this[string key]
        {
            get => _settings[key];
            set => _settings[key] = value;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            var found = _settings.TryGetValue(key, out result);
            if (found)
            {
                return (T) result;
            }
            this[key] = defaultValue;
            return defaultValue;
        }

        public void Remove(string key)
        {
            _settings.Remove(key);
        }
    }
}
