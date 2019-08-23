// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.Forms.Demo.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(ClearCrashes))]
namespace Contoso.Forms.Demo.UWP
{
    public class ClearCrashes : IClearCrashClick
    {
        private const string CrashesUserConfirmationStorageKey = "AppCenterCrashesAlwaysSend";

        public void ClearCrashButton()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(CrashesUserConfirmationStorageKey);
        }
    }
}