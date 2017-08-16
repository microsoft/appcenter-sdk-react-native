using Windows.Storage;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        public object this[string key]
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values[key];
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            var found = ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out result);
            if (found)
            {
                return (T)result;
            }
            this[key] = defaultValue;
            return defaultValue;
        }

        public void Remove(string key)
        {
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }
    }
}
