using System;
using System.Collections.Generic;
using Microsoft.Sonoma.Crashes;
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

        void TestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void DivideByZero(object sender, System.EventArgs e)
        {
            int x = 42 / int.Parse("0");
            x = 0; // to prevent warning
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
