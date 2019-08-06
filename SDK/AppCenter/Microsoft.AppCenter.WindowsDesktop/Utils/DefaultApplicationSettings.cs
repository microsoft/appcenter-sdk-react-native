// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        private const string FileName = "AppCenter.config";
        private static readonly object configLock = new object();
        private static IDictionary<string, string> current;

        internal static string FilePath { get; private set; }

        public DefaultApplicationSettings()
        {
            current = ReadAll();
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            lock (configLock)
            {
                if (current.ContainsKey(key))
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(current[key]);
                }
            }
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
            lock (configLock)
            {
                current[key] = invariant;
                SaveValue(key, invariant);
            }
        }

        public bool ContainsKey(string key)
        {
            lock (configLock)
            {
                return current.ContainsKey(key);
            }
        }

        public void Remove(string key)
        {
            lock (configLock)
            {
                current.Remove(key);
                var config = OpenConfiguration();
                config.AppSettings.Settings.Remove(key);
                config.Save();
            }
        }

        private void SaveValue(string key, string value)
        {
            lock (configLock)
            {
                var config = OpenConfiguration();
                var element = config.AppSettings.Settings[key];
                if (element == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    element.Value = value;
                }
                config.Save();
            }
        }

        private static IDictionary<string, string> ReadAll()
        {
            var config = OpenConfiguration();
            return config.AppSettings.Settings.Cast<KeyValueConfigurationElement>().ToDictionary(e => e.Key, e => e.Value);
        }

        private static Configuration OpenConfiguration()
        {
            // Get new config path.
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var userConfigPath = Path.GetDirectoryName(userConfig.FilePath);

            // Don't have AppCenter.config be reset on each app assembly version, use parent directory.
            var parentDirectory = Path.GetDirectoryName(userConfigPath);
            if (Directory.Exists(parentDirectory))
            {
                userConfigPath = parentDirectory;
            }
            FilePath = Path.Combine(userConfigPath, FileName);

            // If old path exists, migrate.
            try
            {
                // Get old config path.
                var oldLocation = Assembly.GetExecutingAssembly().Location;
                var oldPath = Path.Combine(Path.GetDirectoryName(oldLocation), FileName);
                if (File.Exists(oldPath))
                {
                    // Delete old file if a new one already exists.
                    if (File.Exists(FilePath))
                    {
                        File.Delete(oldPath);
                    }

                    // Or migrate by moving if no new file yet.
                    else
                    {
                        File.Move(oldPath, FilePath);
                    }
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Could not check/migrate old config file", e);
            }

            // Open the configuration (with the new file path).
            var executionFileMap = new ExeConfigurationFileMap { ExeConfigFilename = FilePath };
            return ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
        }
    }
}
