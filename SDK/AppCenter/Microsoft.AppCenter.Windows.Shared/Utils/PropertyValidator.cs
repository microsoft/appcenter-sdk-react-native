// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.AppCenter.Windows.Shared.Utils
{
    public static class PropertyValidator
    {
        internal const int MaxProperties = 20;
        internal const int MaxPropertyKeyLength = 125;
        internal const int MaxPropertyValueLength = 125;

        /// <summary>
        /// Validates properties.
        /// </summary>
        /// <param name="properties">Properties collection to validate.</param>
        /// <param name="logName">Log name.</param>
        /// <returns>Valid properties collection with maximum size of 20.</returns>
        public static IDictionary<string, string> ValidateProperties(IDictionary<string, string> properties, string logName)
        {
            if (properties == null)
            {
                return null;
            }
            var result = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                if (result.Count >= MaxProperties)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : properties cannot contain more than {MaxProperties} items. Skipping other properties.");
                    break;
                }

                // Skip empty property.
                var key = property.Key;
                var value = property.Value;
                if (string.IsNullOrEmpty(key))
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : a property key cannot be null or empty. Property will be skipped.");
                    continue;
                }
                if (value == null)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property value cannot be null. Property will be skipped.");
                    continue;
                }

                // Truncate exceeded property.
                if (key.Length > MaxPropertyKeyLength)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property key length cannot be longer than {MaxPropertyKeyLength} characters. Property key will be truncated.");
                    key = key.Substring(0, MaxPropertyKeyLength);
                }
                if (value.Length > MaxPropertyValueLength)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property value length cannot be longer than {MaxPropertyValueLength} characters. Property value will be truncated.");
                    value = value.Substring(0, MaxPropertyValueLength);
                }
                result.Add(key, value);
            }
            return result;
        }
    }
}
