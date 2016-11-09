using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile;

using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class MobileCenterContentPage : ContentPage
    {
        public MobileCenterContentPage()
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
            MobileCenterEnabledSwitchCell.On = MobileCenter.Enabled;
        }

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (MobileCenterEnabledSwitchCell != null)
            {
                MobileCenter.Enabled = MobileCenterEnabledSwitchCell.On;
            }
        }

      }
}
