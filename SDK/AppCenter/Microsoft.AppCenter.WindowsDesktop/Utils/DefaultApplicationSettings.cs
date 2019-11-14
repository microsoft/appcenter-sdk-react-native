// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        private const string FileName = "AppCenter.config";
        private static readonly object configLock = new object();
        private static Configuration configuration;

        internal static string FilePath { get; private set; }

        public DefaultApplicationSettings()
        {
            lock (configLock)
            {
                configuration = OpenConfiguration();
            }
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            lock (configLock)
            {
                var value = configuration.AppSettings.Settings[key];
                if (value != null)
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value.Value);
                }
            }
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
            lock (configLock)
            {
                SaveValue(key, invariant);
            }
        }

        public bool ContainsKey(string key)
        {
            lock (configLock)
            {
                return configuration.AppSettings.Settings[key] != null;
            }
        }

        public void Remove(string key)
        {
            lock (configLock)
            {
                configuration.AppSettings.Settings.Remove(key);
                SaveConfiguration();
            }
        }

        private void SaveValue(string key, string value)
        {
            lock (configLock)
            {
                var element = configuration.AppSettings.Settings[key];
                if (element == null)
                {
                    configuration.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    element.Value = value;
                }
                SaveConfiguration();
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                configuration.Save();
            }
            catch (ConfigurationErrorsException e)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, $"Configuration file can't be saved. Failure reason: {e.Message}");
            }
        }

        private static Configuration OpenConfiguration()
        {
            // Get new config path.
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var userConfigPath = Path.GetDirectoryName(userConfig.FilePath);

            // Don't have AppCenter.config be reset on each app assembly version, use parent directory.
            var parentDirectory = Path.GetDirectoryName(userConfigPath);
            if (parentDirectory != null)
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
