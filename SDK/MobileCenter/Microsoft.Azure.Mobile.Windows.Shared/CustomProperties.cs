using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.AppCenter
{
    public partial class CustomProperties
    {
        private static readonly Regex KeyPattern = new Regex("^[a-zA-Z][a-zA-Z0-9]*$");
        private const int MaxCustomPropertiesCount = 60;
        private const int MaxCustomPropertiesKeyLength = 128;
        private const int MaxCustomPropertiesStringValueLength = 128;

        internal Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public CustomProperties PlatformSet(string key, string value)
        {
            if (ValidateKey(key))
            {
                if (value == null)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" value cannot be null, did you mean to call clear?");
                }
                else if (value.Length > MaxCustomPropertiesStringValueLength)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" value length cannot be longer than " + MaxCustomPropertiesStringValueLength + " characters.");
                }
                else
                {
                    Properties[key] = value;
                }
            }
            return this;
        }

        public CustomProperties PlatformSet(string key, DateTime value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, int value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, long value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, float value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, double value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, decimal value) => SetObject(key, value);

        public CustomProperties PlatformSet(string key, bool value) => SetObject(key, value);

        public CustomProperties PlatformClear(string key)
        {
            if (ValidateKey(key))
            {

                /* Null value means that key marked to clear. */
                Properties[key] = null;
            }
            return this;
        }

        private CustomProperties SetObject(string key, object value)
        {
            if (ValidateKey(key))
            {
                if (value == null)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" value cannot be null, did you mean to call clear?");
                }
                else
                {
                    Properties[key] = value;
                }
            }
            return this;
        }

        private bool ValidateKey(string key)
        {
            if (key == null || !KeyPattern.IsMatch(key))
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" must match \"" + KeyPattern + "\"");
                return false;
            }
            if (key.Length > MaxCustomPropertiesKeyLength)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" key length cannot be longer than " + MaxCustomPropertiesKeyLength + " characters.");
                return false;
            }
            if (Properties.ContainsKey(key))
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom property \"" + key + "\" is already set or cleared and will be overridden.");
            }
            else if (Properties.Count >= MaxCustomPropertiesCount)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties cannot contain more than " + MaxCustomPropertiesCount + " items.");
                return false;
            }
            return true;
        }
    }
}
