using System;
using System.IO;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;
using System.Linq;
namespace Contoso.Forms.Puppet
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
            int x = (42 / int.Parse("0"));
        }

        void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            Crashes.Enabled = e.Value;
        }

        void GenerateTestCrash(object sender, EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void CatchNullReferenceException(object sender, EventArgs e)
        {
            try
            {
                TriggerNullReferenceException();
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("null reference exception");
            }
        }

        void CrashWithNullReferenceException(object sender, EventArgs e)
        {
            TriggerNullReferenceException();
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
                System.Diagnostics.Debug.WriteLine("{0}{1}", values[ctr].Trim(),
                              ctr == values.GetUpperBound(0) ? "" : ", ");
            System.Diagnostics.Debug.WriteLine("");
        }

        void CrashWithAggregateException(object sender, EventArgs e)
        {
            throw PrepareException();
        }

        static Exception PrepareException()
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

        static Exception SendHttp()
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

        static Exception ValidateLength()
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
