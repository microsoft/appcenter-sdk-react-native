using System;
using System.Collections.Generic;
using Microsoft.Sonoma.Analytics;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public class Property
    {
        public Property(string propertyName, string propertyValue)
        {
            Name = propertyName;
            Value = propertyValue;
        }
        public string Name;
        public string Value;
    }

    public partial class AnalyticsContentPage : ContentPage
    {
        private List<Property> EventProperties;

        public AnalyticsContentPage()
        {
            InitializeComponent();
            EventProperties = new List<Property>();
            NumPropertiesLabel.Text = EventProperties.Count.ToString();
            Analytics.Enabled = true;

            if (Device.OS == TargetPlatform.iOS)
            {
                Icon = "lightning.png";
            }
        }

        void AddProperty(object sender, System.EventArgs e)
        {
            AddPropertyContentPage addPage = new AddPropertyContentPage();
            addPage.PropertyAdded += (Property property) => { 
                EventProperties.Add(property); 
                RefreshPropCount();
            };
            Navigation.PushModalAsync(addPage);
        }

        void PropertiesCellTapped(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PropertiesContentPage(EventProperties));
        }

        void TrackEvent(object sender, System.EventArgs e)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
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

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (EnabledSwitchCell != null)
            {
                Analytics.Enabled = EnabledSwitchCell.On;
            }
        }

        void RefreshPropCount()
        {
            NumPropertiesLabel.Text = EventProperties.Count.ToString();
        }
    }
}
