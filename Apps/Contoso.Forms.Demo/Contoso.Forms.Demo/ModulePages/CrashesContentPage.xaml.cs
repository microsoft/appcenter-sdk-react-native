#define DEBUG

using System;
using System.IO;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class CrashesContentPage
    {
        public CrashesContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.OS == TargetPlatform.iOS)
            {
                Icon = "socket.png";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CrashesEnabledSwitchCell.On = Crashes.Enabled;
            CrashesEnabledSwitchCell.IsEnabled = MobileCenter.Enabled;
        }

        void TestCrash(object sender, EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void DivideByZero(object sender, EventArgs e)
        {
            (42 / int.Parse("0")).ToString();
        }

        void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            Crashes.Enabled = e.Value;
        }

        private void CrashWithNullReferenceException(object sender, EventArgs e)
        {
            TriggerNullReferenceException();
        }

        private void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                System.Diagnostics.Debug.WriteLine("{0}{1}", values[ctr].Trim(),
                              ctr == values.GetUpperBound(0) ? "" : ", ");
            System.Diagnostics.Debug.WriteLine("");
        }

        private void CrashWithAggregateException(object sender, EventArgs e)
        {
            throw PrepareException();
        }

        private static Exception PrepareException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public async void CrashAsync(object sender, EventArgs e)
        {
            await FakeService.DoStuffInBackground();
        }
    }
}
