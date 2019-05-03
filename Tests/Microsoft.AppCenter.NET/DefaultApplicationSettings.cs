// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Application settings implemented in-memory with no persistence.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultApplicationSettings : IApplicationSettings
    {
        private static readonly Dictionary<object, object> Settings = new Dictionary<object, object>();
        
        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            var found = Settings.TryGetValue(key, out result);
            if (found)
            {
                return (T) result;
            }
            SetValue(key, defaultValue);
            return defaultValue;
        }
        public void SetValue(string key, object value)
        {
            Settings[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return Settings.ContainsKey(key);
        }

        public void Remove(string key)
        {
            Settings.Remove(key);
        }

        public static void Reset()
        {
            Settings.Clear();
        }

        public static bool IsEmpty => Settings.Count == 0;
    }
}
