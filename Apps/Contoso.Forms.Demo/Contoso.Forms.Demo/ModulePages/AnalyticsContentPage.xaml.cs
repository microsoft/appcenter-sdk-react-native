// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using System.ComponentModel;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AnalyticsContentPage : ContentPage
    {
        List<Property> EventProperties;

        public AnalyticsContentPage()
        {
            InitializeComponent();
            EventProperties = new List<Property>();
            NumPropertiesLabel.Text = EventProperties.Count.ToString();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "lightning.png";
            }

            // Setup start type dropdown choices
            foreach (var startType in StartTypeUtils.GetStartTypeChoiceStrings())
            {
                this.StartTypePicker.Items.Add(startType);
            }
            this.StartTypePicker.SelectedIndex = (int)(StartTypeUtils.GetPersistedStartType());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            EnabledSwitchCell.On = await Analytics.IsEnabledAsync();
            EnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();
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

        async void AddProperty(object sender, EventArgs e)
        {
            var addPage = new AddPropertyContentPage();
            addPage.PropertyAdded += (Property property) =>
            {
                if (property.Name == null || EventProperties.Any(i => i.Name == property.Name))
                {
                    return;
                }
                EventProperties.Add(property);
                RefreshPropCount();
            };
            await Navigation.PushModalAsync(addPage);
        }

        async void PropertiesCellTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PropertiesContentPage(EventProperties));
        }

        void TrackEvent(object sender, EventArgs e)
        {
            var properties = new Dictionary<string, string>();
            foreach (Property property in EventProperties)
            {
                properties.Add(property.Name, property.Value);
            }

            if (EventProperties.Count == 0)
            {
                Analytics.TrackEvent(EventNameCell.Text);
                return;
            }

            EventProperties.Clear();
            RefreshPropCount();
            Analytics.TrackEvent(EventNameCell.Text, properties);

        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await Analytics.SetEnabledAsync(e.Value);
        }

        void RefreshPropCount()
        {
            NumPropertiesLabel.Text = EventProperties.Count.ToString();
        }
    }
}
