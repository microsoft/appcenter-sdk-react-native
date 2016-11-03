using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class CrashesContentPage : ContentPage
    {
        public CrashesContentPage()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.iOS)
            {
                Icon = "socket.png";
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CrashesEnabledSwitchCell.On = Crashes.Enabled;
        }

        void TestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void DivideByZero(object sender, System.EventArgs e)
        {
            int x = 42 / int.Parse("0");
        }

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (CrashesEnabledSwitchCell != null)
            {
                Crashes.Enabled = CrashesEnabledSwitchCell.On;
            }
        }
    }
}
