using System;
using System.Collections.Generic;
using Microsoft.Sonoma.Core;

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

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (SonomaEnabledSwitchCell != null)
            {
                Sonoma.Enabled = SonomaEnabledSwitchCell.On;
            }
        }

      }
}
