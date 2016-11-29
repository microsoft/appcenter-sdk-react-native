using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Microsoft.Azure.Mobile.Crashes;

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
            throw new DivideByZeroException();
        }
    }
}
