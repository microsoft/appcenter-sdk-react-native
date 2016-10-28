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
        }

        void TestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void DivideByZero(object sender, System.EventArgs e)
        {
            int x = 42 / int.Parse("0");
        }
    }
}
