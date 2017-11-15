using System;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class CrashesController : UITableViewController
    {
        public CrashesController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            CrashesEnabledSwitch.On = Crashes.IsEnabledAsync().Result;
            CrashesEnabledSwitch.Enabled = AppCenter.IsEnabledAsync().Result;
        }

        partial void UpdateEnabled()
        {
            Crashes.SetEnabledAsync(CrashesEnabledSwitch.On).Wait();
            CrashesEnabledSwitch.On = Crashes.IsEnabledAsync().Result;
        }

        partial void TestCrash()
        {
            Crashes.GenerateTestCrash();
        }

        partial void DivideByZero()
        {
            /* This is supposed to cause a crash, so we don't care that the variable 'x' is never used */
#pragma warning disable CS0219
            int x = (42 / int.Parse("0"));
#pragma warning restore CS0219
        }

        partial void CatchNullReferenceException()
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

        partial void CrashWithNullReferenceException()
        {
            TriggerNullReferenceException();
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
            {
                var val = values[ctr].Trim();
                var separator = ctr == values.GetUpperBound(0) ? "" : ", ";
                System.Diagnostics.Debug.WriteLine("{0}{1}", val, separator);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        partial void CrashWithAggregateException()
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

        async partial void CrashAsync()
        {
            await FakeService.DoStuffInBackground();
        }
    }
}
