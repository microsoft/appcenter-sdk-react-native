using System.Collections.Generic;

namespace Microsoft.AppCenter.Windows.Shared.Utils
{
    public static class PropertyValidator
    {
        private const int MaxEventProperties = 20;
        private const int MaxEventPropertyKeyLength = 125;
        private const int MaxEventPropertyValueLength = 125;

        /// <summary>
        /// Validates properties.
        /// </summary>
        /// <param name="properties">Properties collection to validate.</param>
        /// <param name="logName">Log name.</param>
        /// <returns>Valid properties collection with maximum size of 5</returns>
        public static IDictionary<string, string> ValidateProperties(IDictionary<string, string> properties, string logName)
        {
            if (properties == null)
            {
                return null;
            }
            var result = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                if (result.Count >= MaxEventProperties)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : properties cannot contain more than {MaxEventProperties} items. Skipping other properties.");
                    break;
                }

                // Skip empty property.
                var key = property.Key;
                var value = property.Value;
                if (string.IsNullOrEmpty(key))
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : a property key cannot be null or empty. Property will be skipped.");
                    break;
                }
                if (value == null)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property value cannot be null. Property will be skipped.");
                    break;
                }

                // Truncate exceeded property.
                if (key.Length > MaxEventPropertyKeyLength)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property key length cannot be longer than {MaxEventPropertyKeyLength} characters. Property key will be truncated.");
                    key = key.Substring(0, MaxEventPropertyKeyLength);
                }
                if (value.Length > MaxEventPropertyValueLength)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag,
                        $"{logName} : property '{key}' : property value length cannot be longer than {MaxEventPropertyValueLength} characters. Property value will be truncated.");
                    value = value.Substring(0, MaxEventPropertyValueLength);
                }
                result.Add(key, value);
            }
            return result;
        }
    }
}
