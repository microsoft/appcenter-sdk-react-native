using System;
using Foundation;
using Microsoft.Azure.Mobile.iOS.Bindings;

namespace Microsoft.Azure.Mobile
{
    public partial class CustomProperties
    {
        static readonly DateTime _epoch = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal MSCustomProperties IOSCustomProperties { get; } = new MSCustomProperties();

        CustomProperties PlatformSet(string key, string value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, DateTime value)
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate((value.ToUniversalTime() - _epoch).TotalSeconds);
            IOSCustomProperties.Set(nsDate, key);
            return this;
        }


        CustomProperties PlatformSet(string key, int value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, long value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, float value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, double value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, decimal value)
        {
            IOSCustomProperties.Set((double)value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, bool value)
        {
            IOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformClear(string key)
        {
            IOSCustomProperties.Clear(key);
            return this;
        }
    }
}
