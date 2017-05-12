using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly Configuration config;

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
                lock (config)
                {
                    return config.AppSettings.Settings[key]?.Value;
                }
            }
            set
            {
                lock (config)
                {
                    var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
                    var element = config.AppSettings.Settings[key];
                    if (element == null)
                    {
                        config.AppSettings.Settings.Add(key, invariant);
                        config.Save();
                    }
                    else if (element.Value != invariant)
                    {
                        element.Value = invariant;
                        config.Save();
                    }
                }
            }
        }
        public void Remove(string key)
        {
            lock (config)
            {
                config.AppSettings.Settings.Remove(key);
                config.Save();
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            lock (config)
            {
                if (config.AppSettings.Settings[key] != null)
                {
                    var value = config.AppSettings.Settings[key].Value;
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
                }
            }
            this[key] = defaultValue;
            return defaultValue;
        }
    }
}
