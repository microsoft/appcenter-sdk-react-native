// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
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

            // Setup start type dropdown choices.
            foreach (var startType in StartTypeUtils.GetStartTypeChoiceStrings())
            {
                this.StartTypePicker.Items.Add(startType);
            }
            this.StartTypePicker.SelectedIndex = (int)(StartTypeUtils.GetPersistedStartType());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
            if (Application.Current.Properties.ContainsKey(Constants.UserId) && Application.Current.Properties[Constants.UserId] is string id)
            {
                UserIdEntry.Text = id;
            }
            UserIdEntry.Unfocused += async (sender, args) =>
            {
                var inputText = UserIdEntry.Text;
                var text = string.IsNullOrEmpty(inputText) ? null : inputText;
                AppCenter.SetUserId(text);
                Application.Current.Properties[Constants.UserId] = text;
                await Application.Current.SavePropertiesAsync();
            };
        }

        async void ChangeStartType(object sender, PropertyChangedEventArgs e)
        {
            // IOS sends an event every time user rests their selection on an item without hitting "done", and the only event they send when hitting "done" is that the control is no longer focused.
            // So we'll process the change at that time. This works for android as well.
            if (e.PropertyName == "IsFocused" && !this.StartTypePicker.IsFocused)
            {
                var newSelectionCandidate = this.StartTypePicker.SelectedIndex;
                var persistedStartType = StartTypeUtils.GetPersistedStartType();
                if (newSelectionCandidate != (int)persistedStartType)
                {
                    StartTypeUtils.SetPersistedStartType((StartType)newSelectionCandidate);
                    await Application.Current.SavePropertiesAsync();
                    await DisplayAlert("Start Type Changed", "Start type has changed, which alters the app secret. Please close and re-run the app for the new app secret to take effect.", "OK");
                }
            }
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }
    }
}
