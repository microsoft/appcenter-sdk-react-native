using Microsoft.AppCenter.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.AppCenter
{
    public partial class CustomProperties
    {
        private static readonly Regex KeyPattern = new Regex("^[a-zA-Z][a-zA-Z0-9]*$");
        private const int MaxCustomPropertiesCount = 60;
        internal IList<CustomProperty> Properties { get; } = new List<CustomProperty>();

        private CustomProperties SetProperty(CustomProperty property)
        {
            try
            {
                property.Validate();
            }
            catch (ValidationException e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, e.Message);
                return this;
            }
            if (Properties.Count >= MaxCustomPropertiesCount)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties cannot contain more than " + MaxCustomPropertiesCount + " items.");
                return this;
            }
            CustomProperty existingPropertyToRemove = null;
            foreach (var existingProperty in Properties)
            {
                if (existingProperty.Name == property.Name)
                {
                    existingPropertyToRemove = existingProperty;
                    break;
                }
            }
            if (existingPropertyToRemove != null)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Custom property \"" + property.Name + "\" is already set or cleared and will be overwritten.");
                Properties.Remove(existingPropertyToRemove);
            }
            Properties.Add(property);
            return this;
        }

        public CustomProperties PlatformSet(string key, string value)
        {
            return SetProperty(new StringProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, DateTime value)
        {
            return SetProperty(new DateTimeProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, int value)
        {
            return SetProperty(new NumberProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, long value)
        {
            return SetProperty(new NumberProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, float value)
        {
            return SetProperty(new NumberProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, double value)
        {
            return SetProperty(new NumberProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, decimal value)
        {
            return SetProperty(new NumberProperty(key, value));
        }

        public CustomProperties PlatformSet(string key, bool value)
        {
            return SetProperty(new BooleanProperty(key, value));
        }

        public CustomProperties PlatformClear(string key)
        {
            return SetProperty(new ClearProperty(key));
        }
    }
}
