using System;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using System.Linq;

namespace Contoso.Forms.Test
{
    public partial class CrashesPage : ContentPage
    {
        public CrashesPage()
        {
            InitializeComponent();
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void DivideByZeroCrash(object sender, System.EventArgs e)
        {
            Task.Delay(500).Wait();

#pragma warning disable CS0219
            int x = (42 / int.Parse("0"));
#pragma warning restore CS0219
        }

        void GenerateTestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        void CrashWithInvalidOperation(object sender, EventArgs e)
        {
            Task.Delay(500).Wait();

            string[] strings = { "A", "B", "C" };
#pragma warning disable CS0219
            string s = strings.First((arg) => { return arg == "6"; });
#pragma warning restore CS0219
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

        private void CrashWithAggregateException(object sender, EventArgs e)
        {
            Task.Delay(500).Wait();

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
            Task.Delay(500).Wait();

            await FakeService.DoStuffInBackground().ConfigureAwait(false);
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
            {
                System.Diagnostics.Debug.WriteLine("{0}{1}", values[ctr].Trim(),
                              ctr == values.GetUpperBound(0) ? "" : ", ");
            }
            System.Diagnostics.Debug.WriteLine("");
        }
    }

    static class FakeService
    {
        internal static Task DoStuffInBackground()
        {
            return Task.Run(() => { throw new IOException(TestStrings.IOExceptionMessage); });
        }
    }
}
