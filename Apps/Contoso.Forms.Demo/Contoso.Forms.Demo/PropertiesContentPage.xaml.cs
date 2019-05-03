// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class PropertiesContentPage : ContentPage
    {
        public PropertiesContentPage(List<Property> EventProperties)
        {
            InitializeComponent();
            PropertyList.ItemsSource = EventProperties;
        }
    }
}
