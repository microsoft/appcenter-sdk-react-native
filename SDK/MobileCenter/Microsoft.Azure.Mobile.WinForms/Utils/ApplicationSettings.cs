using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}
