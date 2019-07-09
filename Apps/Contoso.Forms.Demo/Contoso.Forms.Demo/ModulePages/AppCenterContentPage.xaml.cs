// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Windows.Input;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AppCenterContentPage : ContentPage
    {
        public AppCenterContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
            UserIdEntry.Unfocused += (sender, args) =>
            {
                var inputText = UserIdEntry.Text;
                var text = string.IsNullOrEmpty(inputText) ? null : inputText;
                AppCenter.SetUserId(text);
                Application.Current.Properties[Constants.UserId] = text;
            };
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }
    }
}
