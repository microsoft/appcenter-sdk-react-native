using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile;

using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class CoreContentPage : ContentPage
    {
        public CoreContentPage()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.iOS)
            {
                Icon = "bolt.png";
            }
        }  

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SonomaEnabledSwitchCell.On = MobileCenter.Enabled;
        }

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (SonomaEnabledSwitchCell != null)
            {
                MobileCenter.Enabled = SonomaEnabledSwitchCell.On;
            }
        }

      }
}
