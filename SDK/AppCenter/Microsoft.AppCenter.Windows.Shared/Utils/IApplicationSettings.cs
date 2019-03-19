// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Represents a store of persistent application settings that is behaves like a dictionary.
    /// </summary>
    public interface IApplicationSettings
    {
        // Returns the object corresponding to 'key'. If there is no such object, it creates one with the given default value, and returns that
        T GetValue<T>(string key, T defaultValue = default(T));
        void SetValue(string key, object value);
        bool ContainsKey(string key);
        void Remove(string key);
    }
}
