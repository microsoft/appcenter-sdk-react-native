using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    class ApplicationSettings : IApplicationSettings
    {
        public object this[string key]
        {
            get
            {
                return Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];
            }

            set
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            bool found = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out result);
            if (!found)
            {
                this[key] = defaultValue;
                return defaultValue;
            }
            return (T)result;
        }
    }
}
