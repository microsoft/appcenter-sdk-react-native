using System;
using Com.Microsoft.Azure.Mobile;
using Java.Lang;
using Java.Util;

namespace Microsoft.Azure.Mobile
{
    public partial class CustomProperties
    {
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal AndroidCustomProperties AndroidCustomProperties { get; } = new AndroidCustomProperties();

        CustomProperties PlatformSet(string key, string value)
        {
            AndroidCustomProperties.Set(key, value);
            return this;
        }

        CustomProperties PlatformSet(string key, DateTime value)
        {
            AndroidCustomProperties.Set(key, new Date((long)(value.ToUniversalTime() - _epoch).TotalMilliseconds));
            return this;
        }

        CustomProperties PlatformSet(string key, int value)
        {
            AndroidCustomProperties.Set(key, new Integer(value));
            return this;
        }

        CustomProperties PlatformSet(string key, long value)
        {
            AndroidCustomProperties.Set(key, new Long(value));
            return this;
        }

        CustomProperties PlatformSet(string key, float value)
        {
            AndroidCustomProperties.Set(key, new Float(value));
            return this;
        }

        CustomProperties PlatformSet(string key, double value)
        {
            AndroidCustomProperties.Set(key, new Java.Lang.Double(value));
            return this;
        }

        CustomProperties PlatformSet(string key, decimal value)
        {
            AndroidCustomProperties.Set(key, new Java.Lang.Double((double)value));
            return this;
        }

        CustomProperties PlatformSet(string key, bool value)
        {
            AndroidCustomProperties.Set(key, value);
            return this;
        }

        CustomProperties PlatformClear(string key)
        {
            AndroidCustomProperties.Clear(key);
            return this;
        }
    }
}
