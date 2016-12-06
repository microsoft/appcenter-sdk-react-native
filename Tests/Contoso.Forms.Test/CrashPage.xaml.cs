using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CrashPage : ContentPage
    {
        public CrashPage()
        {
            InitializeComponent();
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void DivideByZeroCrash(object sender, System.EventArgs e)
        {
            int x = 42 / int.Parse("0");
        }

        void GenerateTestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void CatchNullReferenceException(object sender, EventArgs e)
        {
            try
            {
                TriggerNullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Debug.WriteLine("null reference exception");
            }
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                System.Diagnostics.Debug.WriteLine("{0}{1}", values[ctr].Trim(),
                              ctr == values.GetUpperBound(0) ? "" : ", ");
            System.Diagnostics.Debug.WriteLine("");
        }
    }
}
