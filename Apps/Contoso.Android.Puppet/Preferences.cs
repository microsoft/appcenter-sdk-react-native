// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Android.App;
using Android.Content;

namespace Contoso.Android.Puppet
{
    public static class Preferences
    {
        private const string SharedPreferencesName = "ContosoAppPrefs";

        public static ISharedPreferences SharedPreferences =>
        Application.Context.GetSharedPreferences(SharedPreferencesName, FileCreationMode.Private);
    }
}
