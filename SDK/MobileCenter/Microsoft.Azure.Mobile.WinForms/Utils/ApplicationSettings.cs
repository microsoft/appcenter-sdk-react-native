using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        private Configuration config;

        public ApplicationSettings()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(location), "MobileCenter.config");
            var executionFileMap = new ExeConfigurationFileMap { ExeConfigFilename = path };
            config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
        }

        public object this[string key]
        {
            get
            {
                return config.AppSettings.Settings[key]?.Value;
            }
            set
            {
                var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
                config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, invariant));
                config.Save();
            }
        }
        public void Remove(string key)
        {
            config.AppSettings.Settings.Remove(key);
            config.Save();
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            if (config.AppSettings.Settings[key] != null)
            {
                var value = config.AppSettings.Settings[key].Value;
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
            }
            this[key] = defaultValue;
            return defaultValue;
        }
    }
}
