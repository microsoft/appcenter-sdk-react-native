// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Reflection;
using Microsoft.AppCenter.Utils;
using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Utils
{
    public class ApplicationSettingsTest : IDisposable
    {
        private readonly DefaultApplicationSettings _settings;

        public ApplicationSettingsTest()
        {
            _settings = new DefaultApplicationSettings();
        }

        public void Dispose()
        {
            File.Delete(_settings.FilePath);
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
            Assert.True(_settings.ContainsKey(key));
            Assert.Equal(42, _settings.GetValue<int>(key));
            Assert.Equal(42, _settings.GetValue(key, 0));
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
    }
}
