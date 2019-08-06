// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Utils
{
    public class ApplicationSettingsTest : IDisposable
    {
        private DefaultApplicationSettings _settings;

        public ApplicationSettingsTest()
        {
            _settings = new DefaultApplicationSettings();
        }

        public void Dispose()
        {
            try
            {
                File.Delete(DefaultApplicationSettings.FilePath);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Verify SetValue generic method behaviour
        /// </summary>
        [Fact]
        public void VerifySetValue()
        {
            const string key = "test";
            Assert.False(_settings.ContainsKey(key));
            _settings.SetValue(key, 42);
            Assert.True(_settings.ContainsKey(key));
            Assert.Equal(42, _settings.GetValue<int>(key));
        }

        /// <summary>
        /// Verify GetValue and SetValue generic method behaviour
        /// </summary>
        [Fact]
        public void VerifyGetValue()
        {
            const string key = "test";
            Assert.False(_settings.ContainsKey(key));
            Assert.Equal(42, _settings.GetValue(key, 42));
            Assert.Equal(0, _settings.GetValue<int>(key));
            Assert.Equal(0, _settings.GetValue(key, 0));
        }

        /// <summary>
        /// Verify remove values from settings
        /// </summary>
        [Fact]
        public void VerifyRemove()
        {
            const string key = "test";
            Assert.False(_settings.ContainsKey(key));
            _settings.SetValue(key, 42);
            Assert.True(_settings.ContainsKey(key));
            Assert.Equal(42, _settings.GetValue<int>(key));
            _settings.Remove(key);
            Assert.False(_settings.ContainsKey(key));
        }

        [Fact]
        public void VerifyMigration()
        {
            // Set a value.
            _settings.SetValue("old", "value");

            // Move file back to old location to test migration.
            var oldLocation = typeof(DefaultApplicationSettings).Assembly.Location;
            var oldPath = Path.Combine(Path.GetDirectoryName(oldLocation), "AppCenter.config");
            File.Move(DefaultApplicationSettings.FilePath, oldPath);

            // Migrate.
            _settings = new DefaultApplicationSettings();

            // Check.
            Assert.Equal("value", _settings.GetValue<string>("old"));
        }

        [Fact]
        public void VerifyMigrationSkippedWhenNewFileExists()
        {
            // Set a value.
            _settings.SetValue("key", "oldValue");

            // Move file back to old location to test migration.
            var oldLocation = typeof(DefaultApplicationSettings).Assembly.Location;
            var oldPath = Path.Combine(Path.GetDirectoryName(oldLocation), "AppCenter.config");
            File.Copy(DefaultApplicationSettings.FilePath, oldPath);
            _settings.SetValue("key", "newValue");

            // Migrate.
            _settings = new DefaultApplicationSettings();

            // Check migration didn't happen.
            Assert.Equal("newValue", _settings.GetValue<string>("key"));
        }
    }
}
