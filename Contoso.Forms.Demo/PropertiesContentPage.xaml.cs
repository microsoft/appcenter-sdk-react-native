using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class PropertiesContentPage : ContentPage
    {
        public PropertiesContentPage(List<Property> EventProperties)
        {
            InitializeComponent();
            Title = "Event Properties";

            List<string> properties = new List<string>();

            foreach (Property property in EventProperties)
            {
                string propertyString = property.Name + ": " + property.Value;
                properties.Add(propertyString);
            }

            PropertyList.ItemsSource = properties;
        }
    }
}
