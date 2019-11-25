// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class CustomPropertyView : ContentView
    {
        private enum PropertyTypes { Clear = 0, Boolean, Number, DateTime, String };

        private DatePicker DatePicker;
        private TimePicker TimePicker;

        public CustomPropertyView()
        {
            InitializeComponent();
            InitializePropertyType();
        }

        public void AddCustomProperty(CustomProperties properties)
        {
            if ((PropertyTypes)PropertyType.SelectedIndex == PropertyTypes.Clear)
            {
                properties.Clear(PropertyKey.Text);
            }
            else
            {
                View view = PropertyValueHolder.Children[0];
                switch ((PropertyTypes)PropertyType.SelectedIndex)
                {
                    case PropertyTypes.Boolean:
                        properties.Set(PropertyKey.Text, (view as Switch).IsToggled);
                        break;
                    case PropertyTypes.Number:
                        if (!string.IsNullOrWhiteSpace((view as Editor).Text))
                        {
                            properties.Set(PropertyKey.Text, long.Parse((view as Editor).Text));
                        }
                        break;
                    case PropertyTypes.DateTime:
                        DateTime date = new DateTime(DatePicker.Date.Year, DatePicker.Date.Month, DatePicker.Date.Day,
                                                     TimePicker.Time.Hours, TimePicker.Time.Minutes, TimePicker.Time.Seconds);
                        properties.Set(PropertyKey.Text, date);
                        break;
                    case PropertyTypes.String:
                        properties.Set(PropertyKey.Text, (view as Editor).Text);
                        break;
                }
            }
        }

        private void InitializePropertyType()
        {
            foreach (PropertyTypes propertyType in Enum.GetValues(typeof(PropertyTypes)))
            {
                PropertyType.Items.Add(propertyType.ToString());
            }
            PropertyType.SelectedIndex = 0;
        }

        private void PropertyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyValueHolder.Children.Clear();
            PropertyValue.IsVisible = false;
            switch ((PropertyTypes) PropertyType.SelectedIndex)
            {
                case PropertyTypes.Boolean:
                    PropertyValueHolder.Children.Add(new Switch());
                    PropertyValue.IsVisible = true;
                    break;
                case PropertyTypes.Number:
                    Editor NumberEditor = new Editor() { Keyboard = Keyboard.Numeric };
                    NumberEditor.TextChanged += NumberEditor_TextChanged;
                    PropertyValueHolder.Children.Add(NumberEditor);
                    PropertyValue.IsVisible = true;
                    break;
                case PropertyTypes.DateTime:
                    StackLayout DateTimeLayout = new StackLayout() { Orientation = StackOrientation.Vertical };
                    DatePicker = new DatePicker() { Date = DateTime.Now };
                    DateTimeLayout.Children.Add(DatePicker);
                    TimePicker = new TimePicker() { Time = DateTime.Now.TimeOfDay };
                    DateTimeLayout.Children.Add(TimePicker);
                    PropertyValueHolder.Children.Add(DateTimeLayout);
                    PropertyValue.IsVisible = true;
                    break;
                case PropertyTypes.String:
                    PropertyValueHolder.Children.Add(new Editor());
                    PropertyValue.IsVisible = true;
                    break;
            }
        }

        private void NumberEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                return;
            }
            long newValue;
            if (!long.TryParse(e.NewTextValue, out newValue))
            {
                ((Editor)sender).Text = e.OldTextValue;
            }
        }
    }
}
