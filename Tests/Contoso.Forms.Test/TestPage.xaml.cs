using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        void UnitTestA(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("unit test a");
        }

    }
}
