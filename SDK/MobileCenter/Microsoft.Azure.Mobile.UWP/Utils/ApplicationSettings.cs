namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
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

        public void Remove(string key)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(key);
        }
    }
}
