// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CorePage : ContentPage
    {
        public CorePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (InstallIdLabel != null)
            {
                InstallIdLabel.Text = AppCenter.GetInstallIdAsync().Result?.ToString();
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
